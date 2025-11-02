using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MensagemController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public MensagemController(TechDeskDbContext context)
        {
            _context = context;
        }

        // ✅ POST /api/Mensagem/{chamadoId}
        [HttpPost("{chamadoId:int}")]
        public async Task<IActionResult> EnviarMensagem(int chamadoId, [FromBody] CreateMensagemDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Mensagem))
                return BadRequest("A mensagem não pode estar vazia.");

            var chamadoExiste = await _context.Chamados.AnyAsync(c => c.IdChamado == chamadoId);
            if (!chamadoExiste)
                return NotFound($"Chamado com ID {chamadoId} não encontrado.");

            // 🔒 Garante valores válidos conforme o banco
            string[] statusValidos = { "Aberto", "EmAndamento", "AguardandoCliente", "Fechado" };

            var novaMensagem = new HistoricoChamado
            {
                IdChamado = chamadoId,
                AutorTipo = dto.AutorTipo switch
                {
                    "Usuario" or "Tecnico" or "Sistema" => dto.AutorTipo,
                    _ => "Usuario"
                },
                AutorUsuarioId = dto.AutorUsuarioId,
                AutorTecnicoId = dto.AutorTecnicoId,
                Mensagem = dto.Mensagem,
                Visibilidade = dto.Visibilidade == "Interno" || dto.Visibilidade == "Externo"
                    ? dto.Visibilidade
                    : "Externo",
                StatusAntes = statusValidos.Contains(dto.StatusAntes) ? dto.StatusAntes : "Aberto",
                StatusDepois = statusValidos.Contains(dto.StatusDepois) ? dto.StatusDepois : "EmAndamento",
                Data = DateTime.UtcNow
            };

            _context.HistoricoChamados.Add(novaMensagem);
            await _context.SaveChangesAsync();

            // ✅ Se tiver anexo, cria o registro em AnexosMensagem
            if (!string.IsNullOrEmpty(dto.NomeArquivo))
            {
                var anexo = new AnexosMensagem
                {
                    IdMensagem = novaMensagem.Id,
                    NomeArquivo = dto.NomeArquivo,
                    Url = dto.Url ?? string.Empty,
                    Descricao = dto.DescricaoAnexo ?? string.Empty,
                    ContentType = dto.ContentType ?? "application/octet-stream",
                    TamanhoBytes = dto.TamanhoBytes > 0 ? dto.TamanhoBytes : 0,
                    DataUpload = DateTime.UtcNow
                };

                _context.AnexosMensagems.Add(anexo);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(ListarMensagensPorChamado),
                new { chamadoId = novaMensagem.IdChamado },
                novaMensagem);
        }

        // ✅ PUT /api/Mensagem/{mensagemId}
        [HttpPut("{mensagemId:int}")]
        public async Task<IActionResult> EditarMensagem(int mensagemId, [FromBody] CreateMensagemDTO dto)
        {
            var mensagem = await _context.HistoricoChamados.FindAsync(mensagemId);
            if (mensagem == null)
                return NotFound("Mensagem não encontrada.");

            if (!string.IsNullOrWhiteSpace(dto.Mensagem))
                mensagem.Mensagem = dto.Mensagem;

            await _context.SaveChangesAsync();
            return Ok(mensagem);
        }

        // ✅ GET /api/Mensagem/chamado/{chamadoId}
        [HttpGet("chamado/{chamadoId:int}")]
        public async Task<IActionResult> ListarMensagensPorChamado(int chamadoId)
        {
            var mensagensChamado = await _context.HistoricoChamados
                .Where(h => h.IdChamado == chamadoId)
                .Include(h => h.AutorUsuario)
                .Include(h => h.AutorTecnico)
                .Include(h => h.AnexosMensagems)
                .OrderBy(h => h.Data)
                .Select(h => new
                {
                    h.Id,
                    h.IdChamado,
                    h.Data,
                    h.Mensagem,
                    h.Visibilidade,
                    h.StatusAntes,
                    h.StatusDepois,
                    h.AutorTipo,
                    NomeAutor = h.AutorTipo == "Tecnico"
                        ? h.AutorTecnico != null ? h.AutorTecnico.Nome : null
                        : h.AutorUsuario != null ? h.AutorUsuario.Nome : null,
                    Anexos = h.AnexosMensagems.Select(a => new
                    {
                        a.Id,
                        a.NomeArquivo,
                        a.Url,
                        a.Descricao,
                        a.DataUpload
                    })
                })
                .ToListAsync();

            if (!mensagensChamado.Any())
                return NotFound("Nenhuma mensagem encontrada para este chamado.");

            return Ok(mensagensChamado);
        }
    }
}