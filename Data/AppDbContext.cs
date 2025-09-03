using Microsoft.EntityFrameworkCore;
using IbgeStats.Models;

namespace IbgeStats.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Pesquisa> Pesquisas { get; set; }
        public DbSet<Indicador> Indicadores { get; set; }
        public DbSet<Periodo> Periodos { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<ConsultaLog> ConsultaLogs { get; set; }
        public DbSet<PesquisaStats> PesquisaStats { get; set; }
    }
}