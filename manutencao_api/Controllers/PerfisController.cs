using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using manutencao_api.Models;
using manutencao_api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manutencao_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerfisController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PerfisController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerfilDto>>> GetAll()
        {
            var perfis = await _context.Perfis
                .Select(p => new PerfilDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    NivelAcesso = p.NivelAcesso
                })
                .ToListAsync();

            return Ok(perfis);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PerfilDto>> GetById(int id)
        {
            var perfil = await _context.Perfis
                .Where(p => p.Id == id)
                .Select(p => new PerfilDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    NivelAcesso = p.NivelAcesso
                })
                .FirstOrDefaultAsync();

            if (perfil == null)
                return NotFound(new { Mensagem = "Perfil não encontrado." });

            return Ok(perfil);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePerfilDto dto)
        {
            // Verifica se já existe um perfil com o mesmo nome para evitar duplicidade
            if (await _context.Perfis.AnyAsync(p => p.Nome.ToLower() == dto.Nome.ToLower()))
            {
                return BadRequest(new { Mensagem = "Já existe um perfil registado com este nome." });
            }

            // Verifica se o Nível de Acesso já está em uso (opcional, dependendo da sua regra de negócio)
            if (await _context.Perfis.AnyAsync(p => p.NivelAcesso == dto.NivelAcesso))
            {
                return BadRequest(new { Mensagem = "Este Nível de Acesso já está atribuído a outro perfil." });
            }

            var novoPerfil = new Perfi
            {
                Nome = dto.Nome,
                NivelAcesso = dto.NivelAcesso
            };

            _context.Perfis.Add(novoPerfil);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = novoPerfil.Id }, new { Mensagem = "Perfil criado com sucesso!" });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdatePerfilDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { Mensagem = "O ID da rota não corresponde ao ID do corpo da requisição." });

            var perfil = await _context.Perfis.FindAsync(id);
            if (perfil == null)
                return NotFound(new { Mensagem = "Perfil não encontrado." });

            // Valida conflito de nome com outros perfis
            if (await _context.Perfis.AnyAsync(p => p.Nome.ToLower() == dto.Nome.ToLower() && p.Id != id))
            {
                return BadRequest(new { Mensagem = "Já existe outro perfil registado com este nome." });
            }

            // Valida conflito de nível de acesso com outros perfis
            if (await _context.Perfis.AnyAsync(p => p.NivelAcesso == dto.NivelAcesso && p.Id != id))
            {
                return BadRequest(new { Mensagem = "Este Nível de Acesso já está atribuído a outro perfil." });
            }

            perfil.Nome = dto.Nome;
            perfil.NivelAcesso = dto.NivelAcesso;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var perfil = await _context.Perfis.FindAsync(id);
            if (perfil == null)
                return NotFound(new { Mensagem = "Perfil não encontrado." });

            try
            {
                // Tenta remover fisicamente da base de dados
                _context.Perfis.Remove(perfil);
                await _context.SaveChangesAsync();

                return Ok(new { Mensagem = "Perfil excluído com sucesso." });
            }
            catch (DbUpdateException)
            {
                // Captura o erro de Chave Estrangeira se existirem usuários vinculados a este perfil
                return Conflict(new { Mensagem = "Não é possível excluir este perfil, pois existem utilizadores vinculados a ele." });
            }
        }
    }
}