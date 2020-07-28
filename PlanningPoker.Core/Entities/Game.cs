using System;
using System.Collections.Generic;
using System.Text;

namespace PlanningPoker.Core.Entities
{
    public class Game
    {
        public Guid GameId { get; set; }
        public string GameName { get; set; }
        public DateTime GameTime { get; set; }
        public List<Player> Players { get; set; }
        public List<Card> Cards { get; set; }
    }
}
