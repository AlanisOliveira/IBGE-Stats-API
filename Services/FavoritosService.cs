using Microsoft.EntityFrameworkCore;
using IbgeStats.Data;
using IbgeStats.Models;

namespace IbgeStats.Services
{
    public class FavoritosService : IFavoritosService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FavoritosService> _logger;

        public FavoritosService(AppDbContext context, ILogger<FavoritosService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<UserFavorite>> GetFavoritosByUserAsync(int userId)
        {
            return await _context.UserFavorites
                .Include(f => f.Pesquisa)
                    .ThenInclude(p => p.Stats)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> AdicionarFavoritoAsync(int userId, int pesquisaId)
        {
            try
            {
                // Verificar se já existe
                var existingFavorito = await _context.UserFavorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.PesquisaId == pesquisaId);

                if (existingFavorito != null)
                    return false; // Já existe

                var favorito = new UserFavorite
                {
                    UserId = userId,
                    PesquisaId = pesquisaId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserFavorites.Add(favorito);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar favorito para usuário {UserId} e pesquisa {PesquisaId}", userId, pesquisaId);
                return false;
            }
        }

        public async Task<bool> RemoverFavoritoAsync(int userId, int pesquisaId)
        {
            try
            {
                var favorito = await _context.UserFavorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.PesquisaId == pesquisaId);

                if (favorito == null)
                    return false;

                _context.UserFavorites.Remove(favorito);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover favorito para usuário {UserId} e pesquisa {PesquisaId}", userId, pesquisaId);
                return false;
            }
        }

        public async Task<bool> IsFavoritoAsync(int userId, int pesquisaId)
        {
            return await _context.UserFavorites
                .AnyAsync(f => f.UserId == userId && f.PesquisaId == pesquisaId);
        }

        public async Task<int> GetTotalFavoritosByPesquisaAsync(int pesquisaId)
        {
            return await _context.UserFavorites
                .CountAsync(f => f.PesquisaId == pesquisaId);
        }
    }
}