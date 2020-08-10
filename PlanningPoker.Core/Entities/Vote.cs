using System;

namespace PlanningPoker.Core.Entities
{
    public class Vote
    {
        public Guid VoteId { get; set; }
        public Card Card { get; set; }
        public Player Player { get; set; }
        public int Score { get; set; }
    }
}
