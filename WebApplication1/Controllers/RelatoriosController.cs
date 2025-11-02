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

            return Ok(new
            {
                Periodo = $"{inicio?.ToShortDateString()} - {fim?.ToShortDateString()}",
                TotalChamados = total,
                Abertos = abertos,
                Fechados = fechados,
                MediaSatisfacao = Math.Round(mediaNota, 2)
            });
        }

        [HttpGet("desempenho-tecnicos")]
        public async Task<IActionResult> DesempenhoTecnicos([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            // Carrega técnicos e seus chamados do banco
            var tecnicos = await _context.Tecnicos
                .Include(t => t.Chamados)
                .ToListAsync();

            // Processa em memória para evitar erro de tradução do EF
            var resultadoTecnicos = tecnicos.Select(t => new
            {
                t.Nome,
                ChamadosResolvidos = t.Chamados.Count(c => c.Status == "Fechado"),
                TempoMedioHoras = t.Chamados
                    .Where(c => c.Status == "Fechado" && c.DataFinal != null)
                    .Select(c => (c.DataFinal.Value - c.DataInicio).TotalHours)
                    .DefaultIfEmpty(0)
                    .Average()
            });

            var resultado = new
            {
                Periodo = $"{inicio?.ToShortDateString()} - {fim?.ToShortDateString()}",
                Tecnicos = resultadoTecnicos
            };

            return Ok(resultado);
        }

        // ==================== SATISFAÇÃO DOS CLIENTES ====================
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

            return Ok(new
            {
                Periodo = $"{inicio?.ToShortDateString()} - {fim?.ToShortDateString()}",
                TotalFeedbacks = total,
                MediaGeral = Math.Round(mediaNota, 2),
                UltimosComentarios = ultimosComentarios
            });
        }

        [HttpGet("eficiencia-ia")]
        public async Task<IActionResult> EficienciaIA([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            var chamados = _context.Chamados.AsQueryable();

            // Aplica filtros de período se informados
            if (inicio.HasValue)
                chamados = chamados.Where(c => c.DataInicio >= inicio.Value);
            if (fim.HasValue)
                chamados = chamados.Where(c => c.DataFinal == null || c.DataFinal <= fim.Value);

            // Calcula o tempo médio em minutos (somente chamados finalizados)
            var chamadosFechados = await chamados
                .Where(c => c.DataFinal != null)
                .ToListAsync();

            double tempoMedioMin = 0;
            if (chamadosFechados.Any())
            {
                tempoMedioMin = chamadosFechados
                    .Average(c => (c.DataFinal.Value - c.DataInicio).TotalMinutes);
            }

            // Quantidade de chamados agrupados por status
            var resumoStatus = await chamados
                .GroupBy(c => c.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Quantidade = g.Count()
                })
                .ToListAsync();

            // Monta resultado final
            var resultado = new
            {
                Periodo = $"{inicio?.ToShortDateString()} - {fim?.ToShortDateString()}",
                TempoMedioAtendimentoMin = Math.Round(tempoMedioMin, 2),
                ChamadosPorStatus = resumoStatus
            };

            return Ok(resultado);
        }
    }
}