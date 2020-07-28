using System;

namespace PlanningPoker.Core.Entities
{
    public enum PlayerType
    {
        GameMaster = 0,
        Developer = 1,
        Tester = 2
    }

    public class Player
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
        public PlayerType PlayerType { get; set; }
    }
}
