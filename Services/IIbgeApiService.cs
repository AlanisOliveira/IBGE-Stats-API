using IbgeStats.Models;

namespace IbgeStats.Services
{
    public interface IIbgeApiService
    {
        Task<IEnumerable<Pesquisa>> FetchPesquisasFromIbgeAsync();
        Task<Pesquisa?> FetchPesquisaByIdFromIbgeAsync(int id);
        Task<IEnumerable<Indicador>> FetchIndicadoresByPesquisaAsync(int pesquisaId);
    }
}