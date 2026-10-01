using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using manutencao_api.Models;
using manutencao_api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manutencao_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        // TODO: Substitua 'SeuContexto' pelo nome da sua classe que herda de DbContext
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.PerfilBase)
                .Include(u => u.Setor)
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Telefone = u.Telefone,
                    Ramal = u.Ramal,
                    Cargo = u.Cargo,
                    PerfilBaseId = u.PerfilBaseId,
                    SetorId = u.SetorId,
                    Ativo = u.Ativo,
                    DataCriacao = u.DataCriacao,
                    UltimoLogin = u.UltimoLogin,
                    NomePerfil = u.PerfilBase != null ? u.PerfilBase.Nome : null,
                    NomeSetor = u.Setor != null ? u.Setor.Nome : null
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioDto>> GetById(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.PerfilBase)
                .Include(u => u.Setor)
                .Where(u => u.Id == id)
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Telefone = u.Telefone,
                    Ramal = u.Ramal,
                    Cargo = u.Cargo,
                    PerfilBaseId = u.PerfilBaseId,
                    SetorId = u.SetorId,
                    Ativo = u.Ativo,
                    DataCriacao = u.DataCriacao,
                    UltimoLogin = u.UltimoLogin,
                    NomePerfil = u.PerfilBase != null ? u.PerfilBase.Nome : null,
                    NomeSetor = u.Setor != null ? u.Setor.Nome : null
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound(new { Mensagem = "Usuário não encontrado." });

            return Ok(usuario);
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] CreateUsuarioDto dto)
        {
            // 1. Verifica se o e-mail já existe no banco
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest(new { Mensagem = "Este e-mail já está em uso." });
            }

            // 2. Verifica se o Perfil enviado existe
            var perfilExiste = await _context.Perfis.AnyAsync(p => p.Id == dto.PerfilBaseId);
            if (!perfilExiste)
            {
                return BadRequest(new { Mensagem = "Perfil inválido." });
            }

            // 2.1 Verifica se o Setor existe (caso tenha sido enviado)
            if (dto.SetorId.HasValue)
            {
                var setorExiste = await _context.Setores.AnyAsync(s => s.Id == dto.SetorId.Value);
                if (!setorExiste)
                {
                    return BadRequest(new { Mensagem = "Setor inválido." });
                }
            }

            // 3. Monta o usuário e aplica o HASH na senha usando BCrypt
            var novoUsuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone = dto.Telefone,
                Ramal = dto.Ramal,
                Cargo = dto.Cargo,
                PerfilBaseId = dto.PerfilBaseId,
                SetorId = dto.SetorId,
                Ativo = true, // Forçando true na criação para garantir o acesso inicial
                DataCriacao = DateTime.Now,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
            };

            // 4. Salva no banco de dados
            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Usuário registrado com sucesso!" });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateUsuarioDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { Mensagem = "O ID da rota não corresponde ao ID do corpo da requisição." });

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { Mensagem = "Usuário não encontrado." });

            usuario.Nome = dto.Nome;
            usuario.Email = dto.Email;
            usuario.Telefone = dto.Telefone;
            usuario.Ramal = dto.Ramal;
            usuario.Cargo = dto.Cargo;
            usuario.PerfilBaseId = dto.PerfilBaseId;
            usuario.SetorId = dto.SetorId;
            usuario.Ativo = dto.Ativo;

            // Se uma nova senha for informada no DTO, gera um NOVO HASH e substitui no banco
            if (!string.IsNullOrEmpty(dto.NovaSenha))
            {
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { Mensagem = "Usuário não encontrado." });

            // Eliminação Lógica (Soft Delete)
            usuario.Ativo = false;

            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Usuário inativado com sucesso." });
        }

        // ... dentro de UsuariosController.cs

        [HttpPost("{id:int}/grupos/{grupoId:int}")]
        public async Task<IActionResult> AdicionarAoGrupo(int id, int grupoId)
        {
            var usuario = await _context.Usuarios.Include(u => u.Grupos).FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null) return NotFound(new { Mensagem = "Utilizador não encontrado." });

            var grupo = await _context.Grupos.FindAsync(grupoId); // Supondo que a tabela se chama 'Grupos'
            if (grupo == null) return NotFound(new { Mensagem = "Grupo não encontrado." });

            if (usuario.Grupos.Any(g => g.Id == grupoId))
                return BadRequest(new { Mensagem = "Utilizador já pertence a este grupo." });

            usuario.Grupos.Add(grupo);
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Utilizador adicionado ao grupo com sucesso." });
        }

        [HttpDelete("{id:int}/grupos/{grupoId:int}")]
        public async Task<IActionResult> RemoverDoGrupo(int id, int grupoId)
        {
            var usuario = await _context.Usuarios.Include(u => u.Grupos).FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null) return NotFound(new { Mensagem = "Utilizador não encontrado." });

            var grupo = usuario.Grupos.FirstOrDefault(g => g.Id == grupoId);
            if (grupo == null) return BadRequest(new { Mensagem = "Utilizador não pertence a este grupo." });

            usuario.Grupos.Remove(grupo);
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Utilizador removido do grupo com sucesso." });
        }

        [HttpGet("{id:int}/grupos")]
        public async Task<IActionResult> ObterGruposDoUsuario(int id)
        {
            var grupos = await _context.Usuarios
                .Where(u => u.Id == id)
                .SelectMany(u => u.Grupos)
                .Select(g => new { g.Id, g.Nome }) // Evitar ciclos infinitos de JSON
                .ToListAsync();

            return Ok(grupos);
        }
    }   
}