using IbgeStats.Models;
using System.Text.Json;

namespace IbgeStats.Services
{
    public class IbgeApiService : IIbgeApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IbgeApiService> _logger;
        private const string IbgeBaseUrl = "https://servicodados.ibge.gov.br/api/v3/agregados";

        public IbgeApiService(HttpClient httpClient, ILogger<IbgeApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<Pesquisa>> FetchPesquisasFromIbgeAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(IbgeBaseUrl);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var ibgeData = JsonSerializer.Deserialize<IbgeApiResponse[]>(jsonContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                return ibgeData?.Select(data => new Pesquisa
                {
                    Nome = data.Nome ?? "",
                    Descricao = data.Nome ?? "",
                    Contexto = data.Id?.ToString() ?? "",
                    Categoria = "Geral",
                    LastUpdated = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                }) ?? new List<Pesquisa>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pesquisas da API do IBGE");
                return new List<Pesquisa>();
            }
        }

        public async Task<Pesquisa?> FetchPesquisaByIdFromIbgeAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{IbgeBaseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var ibgeData = JsonSerializer.Deserialize<IbgeApiResponse>(jsonContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                if (ibgeData == null) return null;

                return new Pesquisa
                {
                    Nome = ibgeData.Nome ?? "",
                    Descricao = ibgeData.Nome ?? "",
                    Contexto = ibgeData.Id?.ToString() ?? "",
                    Categoria = "Geral",
                    LastUpdated = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pesquisa {Id} da API do IBGE", id);
                return null;
            }
        }

        public async Task<IEnumerable<Indicador>> FetchIndicadoresByPesquisaAsync(int pesquisaId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{IbgeBaseUrl}/{pesquisaId}/variaveis");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var indicators = JsonSerializer.Deserialize<IndicadorApiResponse[]>(jsonContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                return indicators?.Select(ind => new Indicador
                {
                    PesquisaId = pesquisaId,
                    Nome = ind.Nome ?? "",
                    Unidade = ind.Unidade ?? "",
                    Descricao = ind.Nome ?? "",
                    CreatedAt = DateTime.UtcNow
                }) ?? new List<Indicador>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar indicadores da pesquisa {PesquisaId}", pesquisaId);
                return new List<Indicador>();
            }
        }
    }

    // DTOs para resposta da API do IBGE
    public class IbgeApiResponse
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
    }

    public class IndicadorApiResponse
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
        public string? Unidade { get; set; }
    }
}