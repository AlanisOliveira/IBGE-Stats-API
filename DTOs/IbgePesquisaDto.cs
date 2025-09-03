using System.Text.Json.Serialization;

namespace IbgeStats.DTOs
{
    public class IbgePesquisaDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("contexto")]
        public string Contexto { get; set; } = string.Empty;

        [JsonPropertyName("observacao")]
        public string? Observacao { get; set; }

        [JsonPropertyName("periodos")]
        public List<IbgePeriodoDto>? Periodos { get; set; }
    }
}