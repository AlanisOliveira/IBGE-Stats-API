namespace IbgeStats.Models
{
    public class UserFavorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PesquisaId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public User User { get; set; } = null!;
        public Pesquisa Pesquisa { get; set; } = null!;
    }
}