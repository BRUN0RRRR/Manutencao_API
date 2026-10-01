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
    public class SetoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SetoresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SetorDto>>> GetAll()
        {
            var setores = await _context.Setores
                .Select(s => new SetorDto
                {
                    Id = s.Id,
                    Nome = s.Nome
                })
                .ToListAsync();

            return Ok(setores);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SetorDto>> GetById(int id)
        {
            var setor = await _context.Setores
                .Where(s => s.Id == id)
                .Select(s => new SetorDto
                {
                    Id = s.Id,
                    Nome = s.Nome
                })
                .FirstOrDefaultAsync();

            if (setor == null)
                return NotFound(new { Mensagem = "Setor não encontrado." });

            return Ok(setor);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSetorDto dto)
        {
            // Opcional: Verifica se já existe um setor com o mesmo nome
            if (await _context.Setores.AnyAsync(s => s.Nome.ToLower() == dto.Nome.ToLower()))
            {
                return BadRequest(new { Mensagem = "Já existe um setor registado com este nome." });
            }

            // Repare que instanciamos a classe 'Setore' conforme o seu modelo
            var novoSetor = new Setore
            {
                Nome = dto.Nome
            };

            _context.Setores.Add(novoSetor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = novoSetor.Id }, new { Mensagem = "Setor criado com sucesso!" });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateSetorDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { Mensagem = "O ID da rota não corresponde ao ID do corpo da requisição." });

            var setor = await _context.Setores.FindAsync(id);
            if (setor == null)
                return NotFound(new { Mensagem = "Setor não encontrado." });

            // Verifica se o novo nome já pertence a outro setor diferente
            if (await _context.Setores.AnyAsync(s => s.Nome.ToLower() == dto.Nome.ToLower() && s.Id != id))
            {
                return BadRequest(new { Mensagem = "Já existe outro setor registado com este nome." });
            }

            setor.Nome = dto.Nome;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var setor = await _context.Setores.FindAsync(id);
            if (setor == null)
                return NotFound(new { Mensagem = "Setor não encontrado." });

            try
            {
                // Como não existe campo "Ativo" no Setor, tentamos apagar fisicamente da base de dados
                _context.Setores.Remove(setor);
                await _context.SaveChangesAsync();

                return Ok(new { Mensagem = "Setor excluído com sucesso." });
            }
            catch (DbUpdateException)
            {
                // Se o utilizador tentar excluir um setor que tem Equipamentos ou Utilizadores vinculados,
                // o banco de dados bloqueia (Foreign Key Constraint). Tratamos o erro de forma amigável:
                return Conflict(new { Mensagem = "Não é possível excluir este setor, pois existem equipamentos ou utilizadores vinculados a ele." });
            }
        }
    }
}