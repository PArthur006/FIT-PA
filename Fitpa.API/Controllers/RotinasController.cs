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
    public class RotinasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RotinasController(AppDbContext context)
        {
            _context = context;
        }

        private int ObterUsuarioIdLogado()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) 
                        ?? User.FindFirst("id") 
                        ?? User.FindFirst(ClaimTypes.Name); 
            return int.Parse(claim!.Value);
        }

        [HttpGet]
        public async Task<IActionResult> ListarRotinas()
        {
            var usuarioId = ObterUsuarioIdLogado();

            // Lemos a tabela rotina, mas puxamos os vínculos manualmente
            var rotinas = await _context.Rotinas
                .Where(r => r.UsuarioId == usuarioId)
                .Select(r => new RotinaResponseDto
                {
                    Id = r.Id,
                    Nome = r.Nome,
                    Exercicios = _context.RotinasExercicios
                        .Where(re => re.RotinaId == r.Id)
                        .OrderBy(re => re.Ordem)
                        .Select(re => new RotinaExercicioResponseDto
                        {
                            ExercicioId = re.ExercicioId,
                            NomeExercicio = re.Exercicio.Nome,
                            GrupoMuscular = re.Exercicio.GrupoMuscular,
                            Series = re.Series,
                            Ordem = re.Ordem
                        }).ToList()
                })
                .ToListAsync();

            return Ok(rotinas);
        }

        [HttpPost]
        public async Task<IActionResult> CriarRotina(RotinaCreateDto request)
        {
            var rotina = new Rotina
            {
                Nome = request.Nome,
                UsuarioId = ObterUsuarioIdLogado()
            };
            
            _context.Rotinas.Add(rotina);
            await _context.SaveChangesAsync(); // Precisa salvar a rotina primeiro para ter um Id gerado

            // Agora inserimos as informações na tabela intermediária manualmente
            var vinculos = request.Exercicios.Select(dto => new RotinaExercicio
            {
                RotinaId = rotina.Id,
                ExercicioId = dto.ExercicioId,
                Series = dto.Series,
                Ordem = dto.Ordem
            });

            _context.RotinasExercicios.AddRange(vinculos);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Rotina criada com sucesso" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarRotina(int id)
        {
            var rotina = await _context.Rotinas
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == ObterUsuarioIdLogado());
            
            if (rotina == null) return NotFound("Rotina não encontrada.");

            _context.Rotinas.Remove(rotina);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Rotina deletada com sucesso" });
        }
    }
}