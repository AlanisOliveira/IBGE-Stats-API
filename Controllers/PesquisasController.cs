using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IbgeStats.Models;
using IbgeStats.Services;
using IbgeStats.DTOs;

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
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PesquisaResponseDto>>>> GetPesquisas()
        {
            var pesquisas = await _pesquisaService.GetAllPesquisasAsync();
            var pesquisasDto = pesquisas.Select(MapToResponseDto);
            
            return Ok(ApiResponseDto<IEnumerable<PesquisaResponseDto>>.SuccessResponse(
                pesquisasDto, "Pesquisas encontradas com sucesso"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<PesquisaResponseDto>>> GetPesquisa(int id)
        {
            var pesquisa = await _pesquisaService.GetPesquisaByIdAsync(id);
            
            if (pesquisa == null)
            {
                return NotFound(ApiResponseDto<PesquisaResponseDto>.ErrorResponse("Pesquisa não encontrada"));
            }

            return Ok(ApiResponseDto<PesquisaResponseDto>.SuccessResponse(
                MapToResponseDto(pesquisa), "Pesquisa encontrada com sucesso"));
        }

        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PesquisaResponseDto>>>> GetPesquisasByCategoria(string categoria)
        {
            var pesquisas = await _pesquisaService.GetPesquisasByCategoria(categoria);
            var pesquisasDto = pesquisas.Select(MapToResponseDto);
            
            return Ok(ApiResponseDto<IEnumerable<PesquisaResponseDto>>.SuccessResponse(
                pesquisasDto, $"Pesquisas da categoria '{categoria}' encontradas"));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<PesquisaResponseDto>>> CreatePesquisa(CreatePesquisaDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponseDto<PesquisaResponseDto>.ErrorResponse("Dados inválidos", errors));
            }

            var pesquisa = new Pesquisa
            {
                Nome = createDto.Nome,
                Descricao = createDto.Descricao,
                Contexto = createDto.Contexto,
                Categoria = createDto.Categoria
            };

            var createdPesquisa = await _pesquisaService.CreatePesquisaAsync(pesquisa);
            
            return CreatedAtAction(nameof(GetPesquisa), new { id = createdPesquisa.Id }, 
                ApiResponseDto<PesquisaResponseDto>.SuccessResponse(
                    MapToResponseDto(createdPesquisa), "Pesquisa criada com sucesso"));
        }

        [HttpGet("{id}/indicadores")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<IndicadorResponseDto>>>> GetIndicadoresByPesquisa(int id)
        {
            var pesquisa = await _pesquisaService.GetPesquisaByIdAsync(id);
            if (pesquisa == null)
            {
                return NotFound(ApiResponseDto<IEnumerable<IndicadorResponseDto>>.ErrorResponse("Pesquisa não encontrada"));
            }

            var indicadores = await _ibgeApiService.FetchIndicadoresByPesquisaAsync(id);
            var indicadoresDto = indicadores.Select(i => new IndicadorResponseDto
            {
                Id = i.Id,
                Nome = i.Nome,
                Unidade = i.Unidade,
                Descricao = i.Descricao
            });

            return Ok(ApiResponseDto<IEnumerable<IndicadorResponseDto>>.SuccessResponse(
                indicadoresDto, "Indicadores encontrados com sucesso"));
        }

        [HttpGet("{id}/periodos")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PeriodoResponseDto>>>> GetPeriodosByPesquisa(int id)
        {
            var pesquisa = await _pesquisaService.GetPesquisaByIdAsync(id);
            if (pesquisa == null)
            {
                return NotFound(ApiResponseDto<IEnumerable<PeriodoResponseDto>>.ErrorResponse("Pesquisa não encontrada"));
            }

            var periodosDto = pesquisa.Periodos?.Select(p => new PeriodoResponseDto
            {
                Id = p.Id,
                Ano = p.Ano,
                Versao = p.Versao,
                DataPublicacao = p.DataPublicacao,
                Fontes = p.Fontes,
                Disponivel = p.Disponivel
            }) ?? new List<PeriodoResponseDto>();

            return Ok(ApiResponseDto<IEnumerable<PeriodoResponseDto>>.SuccessResponse(
                periodosDto, "Períodos encontrados com sucesso"));
        }

        [HttpPost("sync-ibge")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> SyncWithIbge()
        {
            var ibgePesquisas = await _ibgeApiService.FetchPesquisasFromIbgeAsync();
            
            foreach (var pesquisa in ibgePesquisas)
            {
                await _pesquisaService.CreatePesquisaAsync(pesquisa);
            }

            return Ok(ApiResponseDto<object>.SuccessResponse(
                new { totalSincronizadas = ibgePesquisas.Count() },
                $"Sincronizadas {ibgePesquisas.Count()} pesquisas do IBGE com sucesso"));
        }

        private static PesquisaResponseDto MapToResponseDto(Pesquisa pesquisa)
        {
            return new PesquisaResponseDto
            {
                Id = pesquisa.Id,
                Nome = pesquisa.Nome,
                Descricao = pesquisa.Descricao,
                Contexto = pesquisa.Contexto,
                Categoria = pesquisa.Categoria,
                LastUpdated = pesquisa.LastUpdated,
                Popularidade = pesquisa.Stats?.ConsultasCount ?? 0,
                Periodos = pesquisa.Periodos?.Select(p => new PeriodoResponseDto
                {
                    Id = p.Id,
                    Ano = p.Ano,
                    Versao = p.Versao,
                    DataPublicacao = p.DataPublicacao,
                    Fontes = p.Fontes,
                    Disponivel = p.Disponivel
                }).ToList() ?? new List<PeriodoResponseDto>(),
                Indicadores = pesquisa.Indicadores?.Select(i => new IndicadorResponseDto
                {
                    Id = i.Id,
                    Nome = i.Nome,
                    Unidade = i.Unidade,
                    Descricao = i.Descricao
                }).ToList() ?? new List<IndicadorResponseDto>()
            };
        }
    }
}