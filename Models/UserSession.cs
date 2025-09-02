namespace IbgeStats.Models
{
    public class UserSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string JwtToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        
        public User User { get; set; } = null!;
    }
}