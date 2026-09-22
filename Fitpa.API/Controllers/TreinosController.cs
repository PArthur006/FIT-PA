using Fitpa.API.Data;
using Fitpa.API.Models;
using Fitpa.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Fitpa.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TreinosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TreinosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarTreino([FromBody] TreinoCreateDto dto)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioIdClaim)) return Unauthorized();

            int usuarioId = int.Parse(usuarioIdClaim);

            var novoTreino = new Treino
            {
                UsuarioId = usuarioId,
                Data = dto.Data.ToUniversalTime(),
                ExerciciosPraticados = new List<TreinoExercicio>()
            };

        int ordemExercicio = 1;
        foreach (var exDto in dto.ExerciciosExecutados)
        {
            var treinoExercicio = new TreinoExercicio
            {
                ExercicioId = exDto.ExercicioId,
                Ordem = ordemExercicio++,
                Series = exDto.Series.Select(s => new TreinoSerie
                {
                    Repeticoes = s.Repeticoes,
                    Peso = s.Peso
                }).ToList()
            };

            novoTreino.ExerciciosPraticados.Add(treinoExercicio);
        }

        _context.Treinos.Add(novoTreino);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Treino registrado com sucesso!", treinoId = novoTreino.Id });
        }
    }
}