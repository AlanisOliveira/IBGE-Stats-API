namespace IbgeStats.Models
{
    public class Indicador
    {
        public int Id { get; set; }
        public int PesquisaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Unidade { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public Pesquisa Pesquisa { get; set; } = null!;
    }
}