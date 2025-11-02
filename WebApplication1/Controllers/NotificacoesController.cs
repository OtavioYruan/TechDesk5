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

        // ✅ GET /me/notificacoes-preferencias?usuarioId=1
        [HttpGet("/me/notificacoes-preferencias")]
        public async Task<IActionResult> GetPreferencias([FromQuery] int usuarioId)
        {
            var prefs = await _context.PreferenciasNotificacaos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

            if (prefs == null)
                return NotFound("Preferências não encontradas para este usuário.");

            return Ok(prefs);
        }

        // ✅ POST /me/notificacoes-preferencias
        [HttpPost("/me/notificacoes-preferencias")]
        public async Task<IActionResult> CriarPreferencias([FromBody] PreferenciasNotificacaoDTO dto)
        {
            if (dto == null)
                return BadRequest("Dados inválidos.");

            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId);
            if (!usuarioExiste)
                return NotFound("Usuário não encontrado.");

            var jaExiste = await _context.PreferenciasNotificacaos.AnyAsync(p => p.UsuarioId == dto.UsuarioId);
            if (jaExiste)
                return Conflict("As preferências desse usuário já existem. Use o PUT para atualizar.");

            var prefs = new PreferenciasNotificacao
            {
                UsuarioId = dto.UsuarioId,
                Email = dto.Email,
                Push = dto.Push,
                StatusUpdates = dto.StatusUpdates,
                AtualizadoEm = DateTime.UtcNow
            };

            _context.PreferenciasNotificacaos.Add(prefs);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPreferencias), new { usuarioId = prefs.UsuarioId }, prefs);
        }

        // ✅ PUT /me/notificacoes-preferencias?usuarioId=1
        [HttpPut("/me/notificacoes-preferencias")]
        public async Task<IActionResult> AtualizarPreferencias([FromQuery] int usuarioId, [FromBody] PreferenciasNotificacaoDTO dto)
        {
            var prefs = await _context.PreferenciasNotificacaos
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

            if (prefs == null)
                return NotFound("Preferências não encontradas para este usuário.");

            prefs.Email = dto.Email;
            prefs.Push = dto.Push;
            prefs.StatusUpdates = dto.StatusUpdates;
            prefs.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Preferências atualizadas com sucesso!",
                prefs
            });
        }
    }
}