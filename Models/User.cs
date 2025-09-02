namespace IbgeStats.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int ConsultasCount { get; set; } = 0;
        public bool IsEmailConfirmed { get; set; } = false;
        
        public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
        public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
        public ICollection<ConsultaLog> ConsultaLogs { get; set; } = new List<ConsultaLog>();
    }
}