namespace IbgeStats.Models
{
    public class Pesquisa
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Contexto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Indicador> Indicadores { get; set; } = new List<Indicador>();
        public ICollection<Periodo> Periodos { get; set; } = new List<Periodo>();
        public ICollection<UserFavorite> Favoritos { get; set; } = new List<UserFavorite>();
        public PesquisaStats? Stats { get; set; }
    }
}