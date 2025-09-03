using IbgeStats.Models;
using IbgeStats.DTOs;
using System.Text.Json;

namespace IbgeStats.Services
{
    public class IbgeApiService : IIbgeApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IbgeApiService> _logger;
        private const string IbgeBaseUrl = "https://servicodados.ibge.gov.br/api/v1/pesquisas";

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
                var ibgePesquisas = JsonSerializer.Deserialize<IbgePesquisaDto[]>(jsonContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                return ibgePesquisas?.Select(dto => new Pesquisa
                {
                    Nome = dto.Descricao ?? "Pesquisa IBGE",
                    Descricao = dto.Observacao ?? dto.Descricao ?? "Pesquisa do IBGE",
                    Contexto = dto.Contexto ?? "",
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
                var ibgePesquisa = JsonSerializer.Deserialize<IbgePesquisaDto>(jsonContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                if (ibgePesquisa == null) return null;

                return new Pesquisa
                {
                    Nome = ibgePesquisa.Descricao ?? "Pesquisa IBGE",
                    Descricao = ibgePesquisa.Observacao ?? ibgePesquisa.Descricao ?? "Pesquisa do IBGE",
                    Contexto = ibgePesquisa.Contexto ?? "",
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
                var response = await _httpClient.GetAsync($"{IbgeBaseUrl}/{pesquisaId}/indicadores/0");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var indicadores = JsonSerializer.Deserialize<IbgeIndicadorDto[]>(jsonContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                return indicadores?.Select(dto => new Indicador
                {
                    PesquisaId = pesquisaId,
                    Nome = dto.Indicador,
                    Unidade = dto.Unidade?.Sufixo ?? "",
                    Descricao = dto.Indicador,
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
}