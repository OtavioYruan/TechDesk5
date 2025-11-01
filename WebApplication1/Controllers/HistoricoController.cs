using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoricoController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public HistoricoController(TechDeskDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<HistoricoChamadoDTO>> Registrar([FromBody] CreateHistoricoDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Mensagem))
                return BadRequest("Dados inválidos. A mensagem é obrigatória.");

            var chamadoExiste = await _context.Chamados.AnyAsync(c => c.IdChamado == dto.IdChamado);
            if (!chamadoExiste)
                return BadRequest($"O chamado com ID {dto.IdChamado} não existe.");

            // 🔹 Valores aceitos pelo banco
            var statusPermitidos = new[] { "Cancelado", "Fechado", "EmAndamento", "Pendente" };

            // 🔹 Normaliza entrada (remove espaços e corrige capitalização)
            dto.StatusDepois = dto.StatusDepois?.Trim();
            dto.StatusAntes = dto.StatusAntes?.Trim();

            // 🔹 Validação de StatusDepois
            if (string.IsNullOrWhiteSpace(dto.StatusDepois) ||
                !statusPermitidos.Contains(dto.StatusDepois, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest($"StatusDepois inválido. Valores aceitos: {string.Join(", ", statusPermitidos)}");
            }

            // 🔹 Validação de StatusAntes (se vier)
            if (!string.IsNullOrWhiteSpace(dto.StatusAntes) &&
                !statusPermitidos.Contains(dto.StatusAntes, StringComparer.OrdinalIgnoreCase) &&
                !dto.StatusAntes.Equals("Aberto", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest($"StatusAntes inválido. Valores aceitos: Aberto, {string.Join(", ", statusPermitidos)}");
            }

            var historico = new HistoricoChamado
            {
                IdChamado = dto.IdChamado,
                Data = DateTime.UtcNow,
                AutorTipo = dto.AutorTipo,
                AutorUsuarioId = dto.AutorUsuarioId,
                AutorTecnicoId = dto.AutorTecnicoId,
                Mensagem = dto.Mensagem,
                Visibilidade = dto.Visibilidade,
                StatusAntes = dto.StatusAntes,
                StatusDepois = dto.StatusDepois
            };

            _context.HistoricoChamados.Add(historico);
            await _context.SaveChangesAsync();

            var dtoResponse = new HistoricoChamadoDTO
            {
                Id = historico.Id,
                IdChamado = historico.IdChamado,
                Data = historico.Data,
                AutorTipo = historico.AutorTipo,
                AutorUsuarioId = historico.AutorUsuarioId,
                AutorTecnicoId = historico.AutorTecnicoId,
                Mensagem = historico.Mensagem,
                Visibilidade = historico.Visibilidade,
                StatusAntes = historico.StatusAntes,
                StatusDepois = historico.StatusDepois,
                NomeAutor = historico.AutorTipo == "Tecnico"
                    ? _context.Tecnicos.FirstOrDefault(t => t.Id == historico.AutorTecnicoId)?.Nome
                    : _context.Usuarios.FirstOrDefault(u => u.Id == historico.AutorUsuarioId)?.Nome
            };

            return CreatedAtAction(nameof(ListarPorChamado), new { chamadoId = dto.IdChamado }, dtoResponse);
        }

        [HttpGet("{chamadoId:int}")]
        public async Task<ActionResult<IEnumerable<HistoricoChamadoDTO>>> ListarPorChamado(int chamadoId)
        {
            var historicos = await _context.HistoricoChamados
                .Where(h => h.IdChamado == chamadoId)
                .OrderByDescending(h => h.Data)
                .Select(h => new HistoricoChamadoDTO
                {
                    Id = h.Id,
                    IdChamado = h.IdChamado,
                    Data = h.Data,
                    AutorTipo = h.AutorTipo,
                    AutorUsuarioId = h.AutorUsuarioId,
                    AutorTecnicoId = h.AutorTecnicoId,
                    Mensagem = h.Mensagem,
                    Visibilidade = h.Visibilidade,
                    StatusAntes = h.StatusAntes,
                    StatusDepois = h.StatusDepois,
                    NomeAutor = h.AutorTipo == "Tecnico"
                        ? h.AutorTecnico != null ? h.AutorTecnico.Nome : null
                        : h.AutorUsuario != null ? h.AutorUsuario.Nome : null
                })
                .ToListAsync();

            if (!historicos.Any())
                return NotFound("Nenhum histórico encontrado para este chamado.");

            return Ok(historicos);
        }
    }
}