using System.Text.Json.Serialization;

namespace IbgeStats.DTOs
{
    public class IbgePeriodoDto
    {
        [JsonPropertyName("periodo")]
        public string Periodo { get; set; } = string.Empty;

        [JsonPropertyName("publicacao")]
        public string Publicacao { get; set; } = string.Empty;

        [JsonPropertyName("versao")]
        public int Versao { get; set; }

        [JsonPropertyName("fonte")]
        public List<string>? Fonte { get; set; }

        [JsonPropertyName("nota")]
        public List<string>? Nota { get; set; }
    }
}