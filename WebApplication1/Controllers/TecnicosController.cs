using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TecnicosController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public TecnicosController(TechDeskDbContext context)
        {
            _context = context;
        }

        // ✅ GET /api/Tecnicos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tecnico>>> GetAll()
        {
            var tecnicos = await _context.Tecnicos.ToListAsync();
            return Ok(tecnicos);
        }

        // ✅ GET /api/Tecnicos/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tecnico>> GetById(int id)
        {
            var tecnico = await _context.Tecnicos.FindAsync(id);
            if (tecnico == null)
                return NotFound(new { mensagem = "Técnico não encontrado" });

            return Ok(tecnico);
        }

        // POST /api/Tecnicos
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CadastroTecnicoDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Nome))
                return BadRequest("Informe nome e e-mail do técnico.");

            // Verifica se já existe técnico com o mesmo e-mail
            bool emailExiste = await _context.Tecnicos.AnyAsync(t => t.Email == dto.Email);
            if (emailExiste)
                return Conflict("E-mail já cadastrado.");

            var novoTecnico = new Tecnico
            {
                Nome = dto.Nome.Trim(),
                Email = dto.Email.Trim(),
                SenhaHash = dto.SenhaHash,
                Perfil = dto.Perfil ?? "Técnico",
                Especialidade = dto.Especialidade,
                Nivel = dto.Nivel,
                CodigoEmpresa = dto.CodigoEmpresa,
                Ativo = dto.Ativo,
                CriadoEm = DateTime.UtcNow
            };

            _context.Tecnicos.Add(novoTecnico);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = novoTecnico.Id }, new
            {
                novoTecnico.Id,
                novoTecnico.Nome,
                novoTecnico.Email,
                novoTecnico.Perfil
            });
        }


        // ✅ PUT /api/Tecnicos/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Tecnico>> Update(int id, [FromBody] Tecnico tecnicoAtualizado)
        {
            var tecnico = await _context.Tecnicos.FindAsync(id);
            if (tecnico == null)
                return NotFound(new { mensagem = "Técnico não encontrado" });

            tecnico.Nome = tecnicoAtualizado.Nome;
            tecnico.Email = tecnicoAtualizado.Email;
            tecnico.Especialidade = tecnicoAtualizado.Especialidade;
            tecnico.Nivel = tecnicoAtualizado.Nivel;
            tecnico.Ativo = tecnicoAtualizado.Ativo;
            tecnico.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(tecnico);
        }

        // ✅ DELETE /api/Tecnicos/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var tecnico = await _context.Tecnicos.FindAsync(id);
            if (tecnico == null)
                return NotFound(new { mensagem = "Técnico não encontrado" });

            _context.Tecnicos.Remove(tecnico);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
