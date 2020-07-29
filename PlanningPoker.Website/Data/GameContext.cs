using Microsoft.EntityFrameworkCore;
using PlanningPoker.Core.Entities;

namespace PlanningPoker.Website.Data
{
    public class GameContext : DbContext
    {
        public GameContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        // Entities
        public DbSet<Game> Games { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Player> Players { get; set; }
    }
}
