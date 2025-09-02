namespace IbgeStats.Models
{
    public class ConsultaLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public string Parameters { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool Success { get; set; } = true;
        
        public User User { get; set; } = null!;
    }
}