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
    public class EquipamentosController : ControllerBase
    {
        // TODO: Substitua 'SeuContexto' pelo nome da sua classe que herda de DbContext
        private readonly AppDbContext _context;

        public EquipamentosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipamentoDto>>> GetAll()
        {
            // Converte a entidade do banco para o DTO de retorno
            var equipamentos = await _context.Equipamentos
                .Include(e => e.Setor) // Inclui os dados do Setor vinculado
                .Select(e => new EquipamentoDto
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    Patrimonio = e.Patrimonio,
                    Status = e.Status,
                    SetorId = e.SetorId,
                    // Pega o nome do setor (Ajuste 'Nome' se a sua classe Setore tiver outro campo para a descrição)
                    NomeSetor = e.Setor != null ? e.Setor.Nome : null
                })
                .ToListAsync();

            return Ok(equipamentos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EquipamentoDto>> GetById(int id)
        {
            var equipamento = await _context.Equipamentos
                .Include(e => e.Setor)
                .Where(e => e.Id == id)
                .Select(e => new EquipamentoDto
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    Patrimonio = e.Patrimonio,
                    Status = e.Status,
                    SetorId = e.SetorId,
                    NomeSetor = e.Setor != null ? e.Setor.Nome : null
                })
                .FirstOrDefaultAsync();

            if (equipamento == null)
                return NotFound(new { message = "Equipamento não encontrado." });

            return Ok(equipamento);
        }

        [HttpPost]
        public async Task<ActionResult<EquipamentoDto>> Create([FromBody] CreateEquipamentoDto dto)
        {
            // Mapeia os dados recebidos do DTO para a Entidade do Banco (sem ID, pois o banco gera)
            var novoEquipamento = new Equipamento
            {
                Nome = dto.Nome,
                Patrimonio = dto.Patrimonio,
                Status = dto.Status,
                SetorId = dto.SetorId
            };

            _context.Equipamentos.Add(novoEquipamento);
            await _context.SaveChangesAsync();

            // Retorna a rota para consultar o item criado
            return CreatedAtAction(nameof(GetById), new { id = novoEquipamento.Id }, novoEquipamento);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateEquipamentoDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "O ID da rota não corresponde ao ID do corpo da requisição." });

            var equipamento = await _context.Equipamentos.FindAsync(id);
            if (equipamento == null)
                return NotFound(new { message = "Equipamento não encontrado." });

            // Atualiza apenas os campos permitidos
            equipamento.Nome = dto.Nome;
            equipamento.Patrimonio = dto.Patrimonio;
            equipamento.Status = dto.Status;
            equipamento.SetorId = dto.SetorId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var equipamento = await _context.Equipamentos.FindAsync(id);
            if (equipamento == null)
                return NotFound(new { message = "Equipamento não encontrado." });

            try
            {
                _context.Equipamentos.Remove(equipamento);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                // Proteção contra a chave estrangeira (diagrama mostra ligação com OrdemServico)
                return Conflict(new { message = "Não é possível excluir o equipamento, pois ele já possui Ordens de Serviço vinculadas." });
            }
        }
    }
}