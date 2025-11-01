using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChamadosController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public ChamadosController(TechDeskDbContext context)
        {
            _context = context;
        }

        // 🔹 GET: api/Chamados
        [HttpGet]
        public async Task<IActionResult> GetChamados()
        {
            var chamados = await _context.Chamados
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.IdTecnicoNavigation)
                .Select(c => new
                {
                    c.IdChamado,
                    c.Titulo,
                    c.Descricao,
                    c.Prioridade,
                    c.Status,
                    c.Nivel,
                    c.DataInicio,
                    c.DataFinal,
                    UsuarioNome = c.IdUsuarioNavigation != null ? c.IdUsuarioNavigation.Nome : null,
                    CategoriaNome = c.IdCategoriaNavigation != null ? c.IdCategoriaNavigation.Nome : null,
                    TecnicoNome = c.IdTecnicoNavigation != null ? c.IdTecnicoNavigation.Nome : null
                })
                .ToListAsync();

            return Ok(chamados);
        }

        // 🔹 GET: api/Chamados/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChamadoPorId(int id)
        {
            var chamado = await _context.Chamados
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.IdTecnicoNavigation)
                .Where(c => c.IdChamado == id)
                .Select(c => new
                {
                    c.IdChamado,
                    c.Titulo,
                    c.Descricao,
                    c.Prioridade,
                    c.Status,
                    c.Nivel,
                    c.DataInicio,
                    c.DataFinal,
                    UsuarioNome = c.IdUsuarioNavigation != null ? c.IdUsuarioNavigation.Nome : null,
                    CategoriaNome = c.IdCategoriaNavigation != null ? c.IdCategoriaNavigation.Nome : null,
                    TecnicoNome = c.IdTecnicoNavigation != null ? c.IdTecnicoNavigation.Nome : null
                })
                .FirstOrDefaultAsync();

            if (chamado == null)
                return NotFound("Chamado não encontrado.");

            return Ok(chamado);
        }

        // 🔹 POST: api/Chamados
        [HttpPost]
        public async Task<IActionResult> CriarChamado([FromBody] CreateChamadoDTO dto)
        {
            if (dto == null)
                return BadRequest("Os dados do chamado são obrigatórios.");

            var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
            if (usuario == null) return BadRequest("Usuário não encontrado.");

            var categoria = await _context.Categorias.FindAsync(dto.IdCategoria);
            if (categoria == null) return BadRequest("Categoria não encontrada.");

            var tecnico = await _context.Tecnicos.FindAsync(dto.IdTecnico);
            if (dto.IdTecnico.HasValue && tecnico == null)
                return BadRequest("Técnico não encontrado.");

            var chamado = new Chamado
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Prioridade = dto.Prioridade,
                Status = "Aberto",
                DataInicio = DateTime.UtcNow,
                IdUsuario = dto.IdUsuario,
                IdCategoria = dto.IdCategoria,
                IdTecnico = dto.IdTecnico,
                Nivel = dto.Nivel
            };

            _context.Chamados.Add(chamado);
            await _context.SaveChangesAsync();

            // Retorno formatado igual ao GET
            var retorno = new
            {
                chamado.IdChamado,
                chamado.Titulo,
                chamado.Descricao,
                chamado.Prioridade,
                chamado.Status,
                chamado.Nivel,
                chamado.DataInicio,
                chamado.DataFinal,
                UsuarioNome = usuario.Nome,
                CategoriaNome = categoria.Nome,
                TecnicoNome = tecnico?.Nome
            };

            return CreatedAtAction(nameof(GetChamadoPorId), new { id = chamado.IdChamado }, retorno);
        }

        // 🔹 PUT: api/Chamados/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarChamado(int id, [FromBody] UpdateChamadoDTO dto)
        {
            var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.IdChamado == id);
            if (chamado == null)
                return NotFound("Chamado não encontrado.");

            chamado.Titulo = dto.Titulo;
            chamado.Descricao = dto.Descricao;
            chamado.Status = dto.Status;
            chamado.Prioridade = dto.Prioridade;
            chamado.IdUsuario = dto.IdUsuario;
            chamado.IdCategoria = dto.IdCategoria;
            chamado.IdTecnico = dto.IdTecnico;
            chamado.Nivel = dto.Nivel;
            chamado.DataFinal = dto.DataFinal;

            await _context.SaveChangesAsync();

            var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
            var categoria = await _context.Categorias.FindAsync(dto.IdCategoria);
            var tecnico = await _context.Tecnicos.FindAsync(dto.IdTecnico);

            var retorno = new
            {
                chamado.IdChamado,
                chamado.Titulo,
                chamado.Descricao,
                chamado.Prioridade,
                chamado.Status,
                chamado.Nivel,
                chamado.DataInicio,
                chamado.DataFinal,
                UsuarioNome = usuario?.Nome,
                CategoriaNome = categoria?.Nome,
                TecnicoNome = tecnico?.Nome
            };

            return Ok(retorno);
        }

        // 🔹 DELETE: api/Chamados/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarChamado(int id)
        {
            var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.IdChamado == id);
            if (chamado == null)
                return NotFound("Chamado não encontrado.");

            _context.Chamados.Remove(chamado);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}