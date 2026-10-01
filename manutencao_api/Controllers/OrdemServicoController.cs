using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Necessário para IFormFile
using System.Security.Claims;
using System.IO; // Necessário para manipulação de pastas e arquivos
using System;
using System.Linq;
using System.Threading.Tasks;
using manutencao_api.DTOs;
using manutencao_api.Models;

namespace manutencao_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // 🔒 Opcional: Adicione [Authorize] aqui se toda a controller exigir JWT
    public class OrdemServicoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdemServicoController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // MÉTODOS ORIGINAIS (Abertura, Listagem e Fechamento)
        // ==========================================

        [HttpPost("abrir")]
        public async Task<IActionResult> AbrirOS([FromBody] NovaOsDTO dto)
        {
            if (dto == null) return BadRequest("Corpo da requisição vazio.");

            int requerenteId = ObterUsuarioId();
            if (requerenteId == 0 && dto.RequerenteId.HasValue)
                requerenteId = dto.RequerenteId.Value;

            if (requerenteId == 0)
                return BadRequest("Requerente não informado e usuário não autenticado.");

            var novaOs = new OrdemServico
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Categoria = dto.Categoria,
                Prioridade = dto.Prioridade,
                Status = "Aberta",
                DataAbertura = DateTime.Now,
                RequerenteId = requerenteId,
                EquipamentoId = dto.EquipamentoId,
                GrupoResponsavelId = dto.GrupoResponsavelId
            };

            _context.OrdemServicos.Add(novaOs);
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Ordem de Serviço aberta com sucesso!", NumeroOS = novaOs.Id });
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarOS()
        {
            var ordens = await _context.OrdemServicos
                .Include(os => os.Requerente)
                .Select(os => new
                {
                    os.Id,
                    os.Titulo,
                    os.Status,
                    os.Prioridade,
                    os.DataAbertura,
                    Requerente = os.Requerente != null ? os.Requerente.Nome : null
                })
                .ToListAsync();

            return Ok(ordens);
        }

        [HttpPut("{id}/fechar")]
        public async Task<IActionResult> FecharOS(int id)
        {
            var os = await _context.OrdemServicos.FindAsync(id);
            if (os == null) return NotFound("OS não encontrada.");
            if (os.Status == "Concluída") return BadRequest("Esta OS já está fechada.");

            os.Status = "Concluída";
            os.DataFechamento = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = $"OS {id} encerrada com sucesso." });
        }

        // ==========================================
        // MÉTODOS DE HISTÓRICO (Comentários e Andamentos)
        // ==========================================

        [HttpPost("{id}/historico")]
        public async Task<IActionResult> AdicionarHistorico(int id, [FromBody] NovoHistoricoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Comentario))
                return BadRequest("O comentário não pode ser vazio.");

            var os = await _context.OrdemServicos.FindAsync(id);
            if (os == null) return NotFound("OS não encontrada.");

            int usuarioId = ObterUsuarioId();
            if (usuarioId == 0) return Unauthorized("Usuário não autenticado.");

            var historico = new HistoricoO
            {
                Comentario = dto.Comentario,
                DataRegistro = DateTime.Now,
                OrdemServicoId = id,
                UsuarioId = usuarioId
            };

            _context.HistoricoOs.Add(historico);
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Histórico adicionado com sucesso." });
        }

        [HttpGet("{id}/historico")]
        public async Task<IActionResult> ListarHistorico(int id)
        {
            var historicos = await _context.HistoricoOs
                .Include(h => h.Usuario)
                .Where(h => h.OrdemServicoId == id)
                .OrderByDescending(h => h.DataRegistro)
                .Select(h => new
                {
                    h.Id,
                    h.Comentario,
                    h.DataRegistro,
                    NomeUsuario = h.Usuario != null ? h.Usuario.Nome : "Desconhecido"
                })
                .ToListAsync();

            return Ok(historicos);
        }

        // ==========================================
        // MÉTODOS DE ANEXOS (Arquivos)
        // ==========================================

        [HttpPost("{id}/anexos")]
        public async Task<IActionResult> UploadAnexo(int id, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhum arquivo foi enviado.");

            var os = await _context.OrdemServicos.FindAsync(id);
            if (os == null) return NotFound("OS não encontrada.");

            int usuarioId = ObterUsuarioId();
            if (usuarioId == 0) return Unauthorized("Usuário não autenticado.");

            // Mude para uma pasta local segura no servidor para evitar problemas de permissão de rede UNC
            var pastaUploads = @"C:\ManutencaoUploads";

            try
            {
                if (!Directory.Exists(pastaUploads))
                {
                    Directory.CreateDirectory(pastaUploads);
                }

                var nomeUnico = $"{Guid.NewGuid()}_{arquivo.FileName}";
                var caminhoCompleto = Path.Combine(pastaUploads, nomeUnico);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }

                var anexo = new OrdemServicoAnexo
                {
                    NomeOriginalArquivo = arquivo.FileName,
                    CaminhoArquivo = caminhoCompleto,
                    TipoArquivo = arquivo.ContentType,
                    TamanhoBytes = (int)arquivo.Length,
                    DataUpload = DateTime.Now,
                    OrdemServicoId = id,
                    UploaderId = usuarioId
                };

                _context.OrdemServicoAnexos.Add(anexo);
                await _context.SaveChangesAsync();

                return Ok(new { Mensagem = "Arquivo anexado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao salvar arquivo: {ex.Message}");
            }
        }

        [HttpGet("{id}/anexos")]
        public async Task<IActionResult> ListarAnexos(int id)
        {
            var anexos = await _context.OrdemServicoAnexos
                .Include(a => a.Uploader)
                .Where(a => a.OrdemServicoId == id)
                .Select(a => new
                {
                    a.Id,
                    a.NomeOriginalArquivo,
                    a.TipoArquivo,
                    TamanhoKB = a.TamanhoBytes / 1024,
                    a.DataUpload,
                    Uploader = a.Uploader != null ? a.Uploader.Nome : "Desconhecido"
                })
                .ToListAsync();

            return Ok(anexos);
        }

        [HttpGet("anexos/{anexoId}/download")]
        public async Task<IActionResult> DownloadAnexo(int anexoId)
        {
            var anexo = await _context.OrdemServicoAnexos.FindAsync(anexoId);
            if (anexo == null) return NotFound("Anexo não encontrado no banco de dados.");

            if (!System.IO.File.Exists(anexo.CaminhoArquivo))
                return NotFound("Arquivo físico não encontrado no servidor.");

            var memoria = new MemoryStream();
            using (var streamFisico = new FileStream(anexo.CaminhoArquivo, FileMode.Open))
            {
                await streamFisico.CopyToAsync(memoria);
            }
            memoria.Position = 0;

            // Retorna o arquivo para download
            return File(memoria, anexo.TipoArquivo, anexo.NomeOriginalArquivo);
        }

        // ==========================================
        // MÉTODOS AUXILIARES
        // ==========================================

        private int ObterUsuarioId()
        {
            // Extrai o ID do usuário diretamente do Token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int id))
            {
                return id;
            }
            return 0;
        }
    }
}