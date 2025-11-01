using Microsoft.AspNetCore.Mvc;
using TechDesk.Models;
using System.Collections.Generic;
using System.Linq;
using TechDesk.Data;
using Microsoft.EntityFrameworkCore;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly TechDeskDbContext _context;
        public CategoriasController(TechDeskDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            return Ok(await _context.Categorias.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CriarCategoria([FromBody] CadastroCategoriaDto dto)
        {
            if (dto == null)
                return BadRequest("Dados inválidos.");

            var categoria = new Categoria
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Ativa = dto.Ativa
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                categoria.IdCategoria,
                categoria.Nome,
                categoria.Descricao,
                categoria.Ativa
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoriaPorId(int id)
        {
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.IdCategoria == id);
            if (categoria == null)
                return NotFound("Categoria não encontrada.");

            return Ok(categoria);
        }
    }
}
