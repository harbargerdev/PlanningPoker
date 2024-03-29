using Microsoft.EntityFrameworkCore;
using PlanningPoker.Core.Entities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace PlanningPoker.Website.Data
{
    public class GameContext : DbContext
    {
        private DbContextOptions<GameContext> options;

        public GameContext(DbContextOptions<GameContext> options)
        {
            this.options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=127.0.0.1;port=3306;Database=planningpoker;uid=planningpoker;pwd=Moxqzqyd47;", ServerVersion.Create(8, 0, 31, ServerType.MySql));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        // Entities
        public DbSet<Game> Games { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Vote> Votes { get; set; }
    }
}
