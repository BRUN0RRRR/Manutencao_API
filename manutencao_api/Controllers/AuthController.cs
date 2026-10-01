using manutencao_api.DTOs;
using manutencao_api.Models;
using manutencao_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// Lembre-se de adicionar o using da sua pasta de Models (ex: using manutencao_api.Models;)

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    // Injeção de dependência do Banco e do Serviço de Token
    public AuthController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registrar([FromBody] RegistroDTO dto)
    {
        // 1. Verifica se o e-mail já existe no banco
        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest(new { Mensagem = "Este e-mail já está em uso." });
        }

        // 2. Verifica se o Perfil enviado existe (ex: 1 = Admin, 2 = Tecnico, 3 = Requerente)
        var perfilExiste = await _context.Perfis.AnyAsync(p => p.Id == dto.PerfilBaseId);
        if (!perfilExiste)
        {
            return BadRequest(new { Mensagem = "Perfil inválido." });
        }

        // 3. Monta o usuário e aplica o HASH na senha usando BCrypt
        var novoUsuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha), // Nunca salva a senha pura!
            PerfilBaseId = dto.PerfilBaseId,
            SetorId = dto.SetorId,
            Ativo = true,
            DataCriacao = DateTime.Now
        };

        // 4. Salva no banco de dados
        _context.Usuarios.Add(novoUsuario);
        await _context.SaveChangesAsync();

        return Ok(new { Mensagem = "Usuário registrado com sucesso!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        // 1. Busca o usuário pelo e-mail e INCLUI os dados do PerfilBase
        var usuario = await _context.Usuarios
            .Include(u => u.PerfilBase) // Importante: Traz a tabela de Perfil junto!
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        // 2. Valida se o usuário existe, se está ativo e se a senha bate com o Hash
        if (usuario == null || !usuario.Ativo || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
        {
            return Unauthorized(new { Mensagem = "E-mail ou senha inválidos." });
        }

        // 3. Atualiza a data do último login (opcional)
        usuario.UltimoLogin = DateTime.Now;
        await _context.SaveChangesAsync();

        // 4. Gera o Token JWT contendo o Perfil
        var token = _tokenService.GerarToken(usuario);

        // 5. Retorna o Token e as informações básicas para o Front-end
        return Ok(new
        {
            Token = token,
            Usuario = new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                Perfil = usuario.PerfilBase.Nome
            }
        });
    }
}