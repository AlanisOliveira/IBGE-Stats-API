namespace IbgeStats.Models
{
    public class PesquisaStats
    {
        public int Id { get; set; }
        public int PesquisaId { get; set; }
        public int ConsultasCount { get; set; } = 0;
        public DateTime? LastAccess { get; set; }
        public int ConsultasHoje { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        public Pesquisa Pesquisa { get; set; } = null!;
    }
}