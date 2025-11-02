using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public FeedbackController(TechDeskDbContext context)
        {
            _context = context;
        }

        // ✅ POST - Registrar novo feedback
        [HttpPost("{chamadoId:int}")]
        public async Task<ActionResult<FeedbackDTO>> EnviarFeedback(int chamadoId, [FromBody] CreateFeedbackDTO dto)
        {
            if (dto == null)
                return BadRequest("Dados inválidos.");

            if (dto.Nota < 1 || dto.Nota > 5)
                return BadRequest("A nota deve ser entre 1 e 5.");

            var chamado = await _context.Chamados
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdChamado == chamadoId);

            if (chamado == null)
                return NotFound($"Chamado com ID {chamadoId} não encontrado.");

            var feedback = new FeedbackAtendimento
            {
                IdChamado = chamadoId,
                UsuarioId = dto.UsuarioId,
                Nota = (byte)dto.Nota,
                Comentario = dto.Comentario,
                Data = DateTime.UtcNow
            };

            _context.FeedbackAtendimentos.Add(feedback);
            await _context.SaveChangesAsync();

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == dto.UsuarioId);

            var response = new FeedbackDTO
            {
                Id = feedback.Id,
                IdChamado = feedback.IdChamado,
                UsuarioId = feedback.UsuarioId,
                Nota = feedback.Nota,
                Comentario = feedback.Comentario,
                Data = feedback.Data,
                NomeUsuario = usuario?.Nome
            };

            return CreatedAtAction(nameof(ObterFeedbackPorChamado),
                new { chamadoId = feedback.IdChamado },
                response);
        }

        // ✅ GET - Listar feedbacks de um chamado
        [HttpGet("{chamadoId:int}")]
        public async Task<IActionResult> ObterFeedbackPorChamado(int chamadoId)
        {
            var lista = await _context.FeedbackAtendimentos
                .AsNoTracking()
                .Where(f => f.IdChamado == chamadoId)
                .Include(f => f.Usuario)
                .Select(f => new FeedbackDTO
                {
                    Id = f.Id,
                    IdChamado = f.IdChamado,
                    UsuarioId = f.UsuarioId,
                    Nota = f.Nota,
                    Comentario = f.Comentario,
                    Data = f.Data,
                    NomeUsuario = f.Usuario != null ? f.Usuario.Nome : null
                })
                .ToListAsync();

            if (!lista.Any())
                return NotFound("Nenhum feedback encontrado para esse chamado.");

            return Ok(lista);
        }
    }
}