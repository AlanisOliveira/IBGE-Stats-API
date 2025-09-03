using IbgeStats.Models;

namespace IbgeStats.Services
{
    public interface IPesquisaService
    {
        Task<IEnumerable<Pesquisa>> GetAllPesquisasAsync();
        Task<Pesquisa?> GetPesquisaByIdAsync(int id);
        Task<Pesquisa> CreatePesquisaAsync(Pesquisa pesquisa);
        Task<Pesquisa?> UpdatePesquisaAsync(int id, Pesquisa pesquisa);
        Task<bool> DeletePesquisaAsync(int id);
        Task<IEnumerable<Pesquisa>> GetPesquisasByCategoria(string categoria);
    }
}