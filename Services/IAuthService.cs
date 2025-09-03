using IbgeStats.DTOs;
using IbgeStats.Models;

namespace IbgeStats.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int userId);
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        bool VerifyPassword(string password, string hash);
        string HashPassword(string password);
    }
}