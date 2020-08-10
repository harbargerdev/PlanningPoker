using Microsoft.EntityFrameworkCore;
using PlanningPoker.Core.Entities;

namespace PlanningPoker.Website.Data
{
    public class GameContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=127.0.0.1;port=3306;Database=planningpoker;uid=planningpoker;pwd=Moxqzqyd47;");
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
