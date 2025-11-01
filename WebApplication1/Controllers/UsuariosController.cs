#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public UsuariosController(TechDeskDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Perfil = u.Perfil,
                    Ativo = u.Ativo,
                    CriadoEm = u.CriadoEm,        // DateTime?
                    AtualizadoEm = u.AtualizadoEm // DateTime?
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // GET: api/Usuarios/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUsuarioPorId(int id)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.Id == id)
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Perfil = u.Perfil,
                    Ativo = u.Ativo,
                    CriadoEm = u.CriadoEm,
                    AtualizadoEm = u.AtualizadoEm
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            return Ok(usuario);
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<IActionResult> CriarUsuario([FromBody] CreateUsuarioDTO dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = dto.SenhaHash,
                Perfil = dto.Perfil,
                Ativo = dto.Ativo,
                CriadoEm = DateTime.UtcNow,
                AtualizadoEm = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var response = new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil,
                Ativo = usuario.Ativo,
                CriadoEm = usuario.CriadoEm,
                AtualizadoEm = usuario.AtualizadoEm
            };

            return CreatedAtAction(nameof(GetUsuarioPorId), new { id = usuario.Id }, response);
        }

        // PUT: api/Usuarios/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> AtualizarUsuario(int id, [FromBody] UpdateUsuarioDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            usuario.Nome = dto.Nome;
            usuario.Email = dto.Email;
            usuario.SenhaHash = dto.SenhaHash;
            usuario.Perfil = dto.Perfil;
            usuario.Ativo = dto.Ativo;
            usuario.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil,
                Ativo = usuario.Ativo,
                CriadoEm = usuario.CriadoEm,
                AtualizadoEm = usuario.AtualizadoEm
            };

            return Ok(response);
        }

        // DELETE: api/Usuarios/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
#nullable restore