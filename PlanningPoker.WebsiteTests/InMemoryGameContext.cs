using Microsoft.EntityFrameworkCore;
using PlanningPoker.Website.Data;

namespace PlanningPoker.WebsiteTests
{
    public class InMemoryGameContext : GameContext
    {
        public InMemoryGameContext(DbContextOptions<GameContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "InMemoryPlanningPokerDb");
        }
    }
}