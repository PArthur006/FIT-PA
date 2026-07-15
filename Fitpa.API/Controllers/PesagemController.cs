using Fitpa.API.Data;
using Fitpa.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fitpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class PesagemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PesagemController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPesagens()
        {
            // Busca todas as pesagens ordenadas pela data em ordem decrescente (Mais recente para a mais antiga)
            var pesagens = await _context.Pesagens
                .OrderByDescending(p => p.Data)
                .ToListAsync();
            return Ok(pesagens);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarPesagem([FromBody] Pesagem pesagem)
        {
            // Bloqueia se a data já existir no banco
            if (_context.Pesagens.Any(p => p.Data == pesagem.Data))
            {
                return BadRequest("Já existe uma pesagem registrada para esta data.");
            }

            // Adiciona o novo registro no banco
            _context.Pesagens.Add(pesagem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPesagens), new {id = pesagem.ID}, pesagem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarPesagem(int id, [FromBody] Pesagem pesagemAtualizada)
        {
            if (id != pesagemAtualizada.ID)
            {
                return BadRequest("O ID da URL não corresponde ao ID do objeto.");
            }

            // Bloqueia se a nova data escolhida já pertencer a outro registro
            if (_context.Pesagens.Any(p => p.Data == pesagemAtualizada.Data && p.ID != id))
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
                if (!_context.Pesagens.Any(e => e.ID == id))
                {
                    return NotFound("Pesagem não encontrada.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Retorna 204 No Content para indicar que a atualização foi bem-sucedida
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarPesagem(int id)
        {
            var pesagem = await _context.Pesagens.FindAsync(id);
            if (pesagem == null)
            {
                return NotFound("Pesagem não encontrada.");
            }

            _context.Pesagens.Remove(pesagem);
            await _context.SaveChangesAsync();

            return NoContent(); // Retorna 204 No Content para indicar que a exclusão foi bem-sucedida
        }
    }
}