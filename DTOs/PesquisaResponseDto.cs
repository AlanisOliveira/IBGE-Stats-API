namespace IbgeStats.DTOs
{
    public class PesquisaResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Contexto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public bool IsFavorito { get; set; } = false;
        public int Popularidade { get; set; } = 0;
        public List<PeriodoResponseDto> Periodos { get; set; } = new();
        public List<IndicadorResponseDto> Indicadores { get; set; } = new();
    }

    public class PeriodoResponseDto
    {
        public int Id { get; set; }
        public string Ano { get; set; } = string.Empty;
        public int Versao { get; set; }
        public DateTime DataPublicacao { get; set; }
        public string Fontes { get; set; } = string.Empty;
        public bool Disponivel { get; set; } = true;
    }

    public class IndicadorResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Unidade { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }
}