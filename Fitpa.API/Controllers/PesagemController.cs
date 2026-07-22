using Fitpa.API.Data;
using Fitpa.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Fitpa.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PesagemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PesagemController(AppDbContext context)
        {
            _context = context;
        }

        /*
         * Consulta
         * Retorna todas as pesagens em ordem decrescente de data.
         */
        [HttpGet]
        public async Task<ActionResult<List<Pesagem>>> GetPesagens()
        {
            var usuarioId = ObterUsuarioIdDoToken();
            return await _context.Pesagens
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.Data)
                .ToListAsync();
        }

        /*
         * Criação
         * Valida data futura e impede duplicidade de registro para a mesma data.
         */
        [HttpPost]
        public async Task<IActionResult> RegistrarPesagem([FromBody] Pesagem pesagem)
        {
            var usuarioId = ObterUsuarioIdDoToken();
            pesagem.UsuarioId = usuarioId;
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            if (pesagem.Data > hoje)
            {
                return BadRequest("Não é possível registrar uma pesagem para uma data futura.");
            }

            /*
             * Regra de unicidade
             * Cada data pode ter apenas um registro de pesagem.
             */
            if (_context.Pesagens.Any(p => p.Data == pesagem.Data))
            {
                return BadRequest("Já existe uma pesagem registrada para esta data.");
            }

            _context.Pesagens.Add(pesagem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPesagens), new { id = pesagem.Id }, pesagem);
        }

        /*
         * Atualização
         * Confere data futura, consistência do Id e duplicidade de data antes de salvar.
         */
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarPesagem(int id, [FromBody] Pesagem pesagemAtualizada)
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            if (pesagemAtualizada.Data > hoje)
            {
                return BadRequest("Não é possível atualizar uma pesagem para uma data futura.");
            }
            if (id != pesagemAtualizada.Id)
            {
                return BadRequest("O ID da URL não corresponde ao ID do objeto.");
            }
            var usuarioId = ObterUsuarioIdDoToken();
            if (!await _context.Pesagens.AnyAsync(p => p.Id == id && p.UsuarioId == usuarioId))
            {
                return Forbid(); // Retorna 403 (Proibido) se tentar mexer na pesagem de outro
            }

            pesagemAtualizada.UsuarioId = usuarioId; // Usa o ID do usuário autenticado

            /*
             * Proteção contra conflito
             * Evita que a nova data colida com outro registro já existente.
             */
            if (_context.Pesagens.Any(p => p.Data == pesagemAtualizada.Data && p.Id != id))
            {
                return BadRequest("Já existe uma pesagem registrada para esta data.");
            }

            _context.Entry(pesagemAtualizada).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Pesagens.Any(e => e.Id == id))
                {
                    return NotFound("Pesagem não encontrada.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /*
         * Exclusão
         * Localiza o registro pelo Id e remove se ele existir.
         */
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarPesagem(int id)
        {
            var pesagem = await _context.Pesagens.FindAsync(id);
            if (pesagem == null)
            {
                return NotFound("Pesagem não encontrada.");
            }
            var usuarioId = ObterUsuarioIdDoToken();
            if (!await _context.Pesagens.AnyAsync(p => p.Id == id && p.UsuarioId == usuarioId))
            {
                return Forbid(); // Retorna 403 (Proibido) se tentar mexer na pesagem de outro
            }

            _context.Pesagens.Remove(pesagem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int ObterUsuarioIdDoToken()
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claimId!);
        }
    }
}