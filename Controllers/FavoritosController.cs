using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IbgeStats.DTOs;
using IbgeStats.Services;
using IbgeStats.Models;

namespace IbgeStats.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritosController : ControllerBase
    {
        private readonly IPesquisaService _pesquisaService;
        private readonly IFavoritosService _favoritosService;

        public FavoritosController(IPesquisaService pesquisaService, IFavoritosService favoritosService)
        {
            _pesquisaService = pesquisaService;
            _favoritosService = favoritosService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PesquisaResponseDto>>>> GetMeusFavoritos()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(ApiResponseDto<IEnumerable<PesquisaResponseDto>>.ErrorResponse("Token inválido"));

            var favoritos = await _favoritosService.GetFavoritosByUserAsync(userId.Value);
            var favoritosDto = favoritos.Select(f => new PesquisaResponseDto
            {
                Id = f.Pesquisa.Id,
                Nome = f.Pesquisa.Nome,
                Descricao = f.Pesquisa.Descricao,
                Contexto = f.Pesquisa.Contexto,
                Categoria = f.Pesquisa.Categoria,
                LastUpdated = f.Pesquisa.LastUpdated,
                IsFavorito = true,
                Popularidade = f.Pesquisa.Stats?.ConsultasCount ?? 0
            });

            return Ok(ApiResponseDto<IEnumerable<PesquisaResponseDto>>.SuccessResponse(
                favoritosDto, "Favoritos encontrados com sucesso"));
        }

        [HttpPost("{pesquisaId}")]
        public async Task<ActionResult<ApiResponseDto<object>>> AdicionarFavorito(int pesquisaId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(ApiResponseDto<object>.ErrorResponse("Token inválido"));

            // Verificar se pesquisa existe
            var pesquisa = await _pesquisaService.GetPesquisaByIdAsync(pesquisaId);
            if (pesquisa == null)
                return NotFound(ApiResponseDto<object>.ErrorResponse("Pesquisa não encontrada"));

            var result = await _favoritosService.AdicionarFavoritoAsync(userId.Value, pesquisaId);
            if (!result)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Pesquisa já está nos favoritos"));

            return Ok(ApiResponseDto<object>.SuccessResponse(
                new { pesquisaId, isFavorito = true }, 
                "Pesquisa adicionada aos favoritos"));
        }

        [HttpDelete("{pesquisaId}")]
        public async Task<ActionResult<ApiResponseDto<object>>> RemoverFavorito(int pesquisaId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(ApiResponseDto<object>.ErrorResponse("Token inválido"));

            var result = await _favoritosService.RemoverFavoritoAsync(userId.Value, pesquisaId);
            if (!result)
                return NotFound(ApiResponseDto<object>.ErrorResponse("Favorito não encontrado"));

            return Ok(ApiResponseDto<object>.SuccessResponse(
                new { pesquisaId, isFavorito = false }, 
                "Pesquisa removida dos favoritos"));
        }

        [HttpGet("{pesquisaId}/status")]
        public async Task<ActionResult<ApiResponseDto<object>>> VerificarFavorito(int pesquisaId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(ApiResponseDto<object>.ErrorResponse("Token inválido"));

            var isFavorito = await _favoritosService.IsFavoritoAsync(userId.Value, pesquisaId);

            return Ok(ApiResponseDto<object>.SuccessResponse(
                new { pesquisaId, isFavorito }, 
                "Status do favorito verificado"));
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;
            return null;
        }
    }
}