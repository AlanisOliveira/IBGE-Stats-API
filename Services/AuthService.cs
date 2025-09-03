using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IbgeStats.Data;
using IbgeStats.DTOs;
using IbgeStats.Models;

namespace IbgeStats.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Verificar se usuário já existe
                if (await GetUserByEmailAsync(registerDto.Email) != null)
                {
                    return null; // Usuário já existe
                }

                // Criar novo usuário
                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = HashPassword(registerDto.Password),
                    Role = "User",
                    CreatedAt = DateTime.UtcNow,
                    IsEmailConfirmed = true // Simplificado para demo
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Gerar tokens
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Salvar sessão
                var session = new UserSession
                {
                    UserId = user.Id,
                    JwtToken = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserSessions.Add(session);
                await _context.SaveChangesAsync();

                return new AuthResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = session.ExpiresAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário {Email}", registerDto.Email);
                return null;
            }
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await GetUserByEmailAsync(loginDto.Email);
                if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    return null;
                }

                // Gerar tokens
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Salvar sessão
                var session = new UserSession
                {
                    UserId = user.Id,
                    JwtToken = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserSessions.Add(session);
                await _context.SaveChangesAsync();

                return new AuthResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = session.ExpiresAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer login do usuário {Email}", loginDto.Email);
                return null;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "MinhaChaveSecretaMuitoSeguraParaJWT12345");
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["JwtSettings:Issuer"] ?? "IbgeStatsAPI",
                Audience = _configuration["JwtSettings:Audience"] ?? "IbgeStatsAPI"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}