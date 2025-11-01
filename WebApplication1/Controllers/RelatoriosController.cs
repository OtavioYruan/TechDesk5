using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public RelatoriosController(TechDeskDbContext context)
        {
            _context = context;
        }

        // ==================== RELATÓRIO GERAL ====================
        // GET /relatorios/visao-geral?inicio=2025-01-01&fim=2025-12-31
        [HttpGet("visao-geral")]
        public async Task<IActionResult> VisaoGeral([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            var chamados = _context.Chamados.AsQueryable();

            if (inicio.HasValue)
                chamados = chamados.Where(c => c.DataInicio >= inicio.Value);
            if (fim.HasValue)
                chamados = chamados.Where(c => c.DataFinal == null || c.DataFinal <= fim.Value);

            var total = await chamados.CountAsync();
            var abertos = await chamados.CountAsync(c => c.Status == "Aberto");
            var fechados = await chamados.CountAsync(c => c.Status == "Fechado");

            var mediaNota = await _context.FeedbackAtendimentos.AnyAsync()
                ? await _context.FeedbackAtendimentos.AverageAsync(f => f.Nota)
                : 0;

            var resultado = new
            {
                Periodo = $"{inicio?.ToShortDateString()} - {fim?.ToShortDateString()}",
                TotalChamados = total,
                Abertos = abertos,
                Fechados = fechados,
                MediaSatisfacao = Math.Round(mediaNota, 2)
            };

            return Ok(resultado);
        }

        // ==================== DESEMPENHO DOS TÉCNICOS ====================
        // GET /relatorios/desempenho-tecnicos?inicio=2025-01-01&fim=2025-12-31
        [HttpGet("desempenho-tecnicos")]
        public async Task<IActionResult> DesempenhoTecnicos([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            var tecnicos = await _context.Tecnicos
                .Include(t => t.Chamados)
                .Select(t => new
                {
                    t.Nome,
                    ChamadosResolvidos = t.Chamados.Count(c => c.Status == "Fechado"),
                    TempoMedioHoras = t.Chamados
                        .Where(c => c.DataFinal != null)
                        .Select(c => EF.Functions.DateDiffMinute(c.DataInicio, c.DataFinal.Value) / 60.0)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .ToListAsync();

            var resultado = new
            {
                Periodo = $"{inicio?.ToShortDateString()} - {fim?.ToShortDateString()}",
                Tecnicos = tecnicos
            };

            return Ok(resultado);
        }

        // ==================== SATISFAÇÃO DOS CLIENTES ====================
        // GET /relatorios/satisfacao-clientes?inicio=2025-01-01&fim=2025-12-31
        [HttpGet("satisfacao-clientes")]
        public async Task<IActionResult> SatisfacaoClientes([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            var feedbacks = _context.FeedbackAtendimentos.AsQueryable();

            if (inicio.HasValue)
                feedbacks = feedbacks.Where(f => f.Data >= inicio.Value);
            if (fim.HasValue)
                feedbacks = feedbacks.Where(f => f.Data <= fim.Value);

            var total = await feedbacks.CountAsync();
            var mediaNota = total > 0 ? await feedbacks.AverageAsync(f => f.Nota) : 0;

            var ultimosComentarios = await feedbacks
                .OrderByDescending(f => f.Data)
                .Take(5)
                .Select(f => new { f.Nota, f.Comentario, f.Data })
                .ToListAsync();

            var resultado = new
            {
                Periodo = $"{inicio?.ToShortDateString()} - {fim?.ToShortDateString()}",
                TotalFeedbacks = total,
                MediaGeral = Math.Round(mediaNota, 2),
                UltimosComentarios = ultimosComentarios
            };

            return Ok(resultado);
        }

        // ==================== EFICIÊNCIA DA IA ====================
        // GET /relatorios/eficiencia-ia?inicio=2025-01-01&fim=2025-12-31
        [HttpGet("eficiencia-ia")]
        public async Task<IActionResult> EficienciaIA([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            var eficiencia = await _context.VwTmas.FirstOrDefaultAsync();
            var resumoStatus = await _context.VwResumoPorStatuses.ToListAsync();

            var resultado = new
            {
                Periodo = $"{inicio?.ToShortDateString()} - {fim?.ToShortDateString()}",
                TempoMedioAtendimentoMin = eficiencia?.TempoMedioMin ?? 0,
                ChamadosPorStatus = resumoStatus
            };

            return Ok(resultado);
        }
    }
}
