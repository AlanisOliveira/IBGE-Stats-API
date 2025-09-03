using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IbgeStats.DTOs;
using IbgeStats.Services;

namespace IbgeStats.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponseDto<AuthResponseDto>.ErrorResponse("Dados inválidos", errors));
            }

            var result = await _authService.RegisterAsync(registerDto);
            if (result == null)
            {
                return BadRequest(ApiResponseDto<AuthResponseDto>.ErrorResponse("Usuário já existe ou erro interno"));
            }

            return Ok(ApiResponseDto<AuthResponseDto>.SuccessResponse(result, "Usuário registrado com sucesso"));
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponseDto<AuthResponseDto>.ErrorResponse("Dados inválidos", errors));
            }

            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
            {
                return Unauthorized(ApiResponseDto<AuthResponseDto>.ErrorResponse("Email ou senha inválidos"));
            }

            return Ok(ApiResponseDto<AuthResponseDto>.SuccessResponse(result, "Login realizado com sucesso"));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(ApiResponseDto<UserResponseDto>.ErrorResponse("Token inválido"));
            }

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponseDto<UserResponseDto>.ErrorResponse("Usuário não encontrado"));
            }

            var userDto = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                ConsultasCount = user.ConsultasCount,
                IsEmailConfirmed = user.IsEmailConfirmed
            };

            return Ok(ApiResponseDto<UserResponseDto>.SuccessResponse(userDto, "Dados do usuário obtidos com sucesso"));
        }
    }
}