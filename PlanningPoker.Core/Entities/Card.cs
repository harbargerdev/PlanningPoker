using System;
using System.Collections.Generic;
using System.Text;

namespace PlanningPoker.Core.Entities
{
    public class Card
    {
        public Guid CardId { get; set; }
        public string CardSource { get; set; }
        public string CardNumber { get; set; }
        public int DeveloperSize { get; set; }
        public int DeveloperVotes { get; set; }
        public int TestingSize { get; set; }
        public int TestingVotes { get; set; }
        public int StorySize { get; set; }
        public bool IsLocked { get; set; }
        public bool IsFinished { get; set; }
        public List<Vote> Votes { get; set; }
    }
}
