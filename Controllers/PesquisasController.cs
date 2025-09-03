using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IbgeStats.Data;
using IbgeStats.Models;

namespace IbgeStats.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PesquisasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PesquisasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pesquisa>>> GetPesquisas()
        {
            var pesquisas = await _context.Pesquisas
                .Include(p => p.Indicadores)
                .Include(p => p.Periodos)
                .Include(p => p.Stats)
                .ToListAsync();

            return Ok(pesquisas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pesquisa>> GetPesquisa(int id)
        {
            var pesquisa = await _context.Pesquisas
                .Include(p => p.Indicadores)
                .Include(p => p.Periodos)
                .Include(p => p.Stats)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pesquisa == null)
            {
                return NotFound();
            }

            return Ok(pesquisa);
        }

        [HttpPost]
        public async Task<ActionResult<Pesquisa>> CreatePesquisa(Pesquisa pesquisa)
        {
            _context.Pesquisas.Add(pesquisa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPesquisa), new { id = pesquisa.Id }, pesquisa);
        }
    }
}