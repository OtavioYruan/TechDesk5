using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacoesController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public NotificacoesController(TechDeskDbContext context)
        {
            _context = context;
        }

        // GET /me/notificacoes-preferencias?usuarioId=1
        [HttpGet("/me/notificacoes-preferencias")]
        public async Task<IActionResult> GetPreferencias([FromQuery] int usuarioId)
        {
            var prefs = await _context.PreferenciasNotificacaos
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

            if (prefs == null)
                return NotFound("Preferências não encontradas para este usuário.");

            return Ok(prefs);
        }

        // PUT /me/notificacoes-preferencias?usuarioId=1
        [HttpPut("/me/notificacoes-preferencias")]
        public async Task<IActionResult> AtualizarPreferencias(
            [FromQuery] int usuarioId,
            [FromBody] PreferenciasNotificacao novasPrefs)
        {
            var prefs = await _context.PreferenciasNotificacaos
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

            if (prefs == null)
                return NotFound("Preferências não encontradas para este usuário.");

            prefs.Email = novasPrefs.Email;
            prefs.Push = novasPrefs.Push;
            prefs.StatusUpdates = novasPrefs.StatusUpdates;
            prefs.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Preferências atualizadas com sucesso!",
                prefs
            });
        }

        // POST /notificacoes/testar
        [HttpPost("/notificacoes/testar")]
        public IActionResult EnviarNotificacaoTeste([FromBody] string destino)
        {
            if (string.IsNullOrWhiteSpace(destino))
                return BadRequest("Destino não pode ser vazio.");

            return Ok(new
            {
                mensagem = $"Notificação de teste enviada para {destino}.",
                dataEnvio = DateTime.UtcNow
            });
        }
    }
}
