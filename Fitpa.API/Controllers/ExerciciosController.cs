using Fitpa.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fitpa.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciciosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExerciciosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos()
        {
            var exercicios = await _context.Exercicios
                .Select(e => new { e.Id, e.Nome, e.GrupoMuscular })
                .OrderBy(e => e.GrupoMuscular).ThenBy(e => e.Nome)
                .ToListAsync();
            return Ok(exercicios);
        }
    }
}