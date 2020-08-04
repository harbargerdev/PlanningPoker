using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace PlanningPoker.Website.Hubs
{
    public class VoteHub : Hub
    {
        public async Task SendMessage(Guid gameId, string votingState)
        {
            await Clients.All.SendAsync("VotingStatus", gameId, votingState);
        }
    }
}
