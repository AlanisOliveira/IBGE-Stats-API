using Microsoft.AspNetCore.Mvc;
using IbgeStats.Models;
using IbgeStats.Services;

namespace IbgeStats.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PesquisasController : ControllerBase
    {
        private readonly IPesquisaService _pesquisaService;
        private readonly IIbgeApiService _ibgeApiService;

        public PesquisasController(IPesquisaService pesquisaService, IIbgeApiService ibgeApiService)
        {
            _pesquisaService = pesquisaService;
            _ibgeApiService = ibgeApiService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pesquisa>>> GetPesquisas()
        {
            var pesquisas = await _pesquisaService.GetAllPesquisasAsync();
            return Ok(pesquisas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pesquisa>> GetPesquisa(int id)
        {
            var pesquisa = await _pesquisaService.GetPesquisaByIdAsync(id);
            
            if (pesquisa == null)
            {
                return NotFound();
            }

            return Ok(pesquisa);
        }

        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<IEnumerable<Pesquisa>>> GetPesquisasByCategoria(string categoria)
        {
            var pesquisas = await _pesquisaService.GetPesquisasByCategoria(categoria);
            return Ok(pesquisas);
        }

        [HttpPost]
        public async Task<ActionResult<Pesquisa>> CreatePesquisa(Pesquisa pesquisa)
        {
            var createdPesquisa = await _pesquisaService.CreatePesquisaAsync(pesquisa);
            return CreatedAtAction(nameof(GetPesquisa), new { id = createdPesquisa.Id }, createdPesquisa);
        }

        [HttpPost("sync-ibge")]
        public async Task<ActionResult> SyncWithIbge()
        {
            var ibgePesquisas = await _ibgeApiService.FetchPesquisasFromIbgeAsync();
            
            foreach (var pesquisa in ibgePesquisas)
            {
                await _pesquisaService.CreatePesquisaAsync(pesquisa);
            }

            return Ok(new { message = $"Sincronizadas {ibgePesquisas.Count()} pesquisas do IBGE" });
        }
    }
}