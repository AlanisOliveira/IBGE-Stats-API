namespace IbgeStats.Models
{
    public class Periodo
    {
        public int Id { get; set; }
        public int PesquisaId { get; set; }
        public string Ano { get; set; } = string.Empty;
        public int Versao { get; set; }
        public DateTime DataPublicacao { get; set; }
        public string Fontes { get; set; } = string.Empty;
        public bool Disponivel { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public Pesquisa Pesquisa { get; set; } = null!;
    }
}