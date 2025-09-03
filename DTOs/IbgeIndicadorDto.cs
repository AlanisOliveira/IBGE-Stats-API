using System.Text.Json.Serialization;

namespace IbgeStats.DTOs
{
    public class IbgeIndicadorDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("posicao")]
        public string Posicao { get; set; } = string.Empty;

        [JsonPropertyName("classe")]
        public string? Classe { get; set; }

        [JsonPropertyName("indicador")]
        public string Indicador { get; set; } = string.Empty;

        [JsonPropertyName("unidade")]
        public IbgeUnidadeDto? Unidade { get; set; }

        [JsonPropertyName("children")]
        public List<IbgeIndicadorDto>? Children { get; set; }

        [JsonPropertyName("nota")]
        public List<object>? Nota { get; set; }
    }

    public class IbgeUnidadeDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("classe")]
        public string Classe { get; set; } = string.Empty;

        [JsonPropertyName("multiplicador")]
        public decimal Multiplicador { get; set; }

        [JsonPropertyName("proporcao")]
        public decimal Proporcao { get; set; }

        [JsonPropertyName("sufixo")]
        public string Sufixo { get; set; } = string.Empty;
    }
}