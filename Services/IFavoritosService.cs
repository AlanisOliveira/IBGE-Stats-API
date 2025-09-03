using IbgeStats.Models;

namespace IbgeStats.Services
{
    public interface IFavoritosService
    {
        Task<IEnumerable<UserFavorite>> GetFavoritosByUserAsync(int userId);
        Task<bool> AdicionarFavoritoAsync(int userId, int pesquisaId);
        Task<bool> RemoverFavoritoAsync(int userId, int pesquisaId);
        Task<bool> IsFavoritoAsync(int userId, int pesquisaId);
        Task<int> GetTotalFavoritosByPesquisaAsync(int pesquisaId);
    }
}