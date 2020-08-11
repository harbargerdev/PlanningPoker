using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using PlanningPoker.Cleaner.Data;
using PlanningPoker.Core.Entities;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PlanningPoker.Cleaner
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            // Initialize count
            int count = 0;

            var _context = new GameContext();
            var targetDate = DateTime.Now.AddDays(-7);
            
            // Get List of games older than yesterday
            var games = _context.Games
                .Include(g => g.Cards)
                .ThenInclude(c => c.Votes)
                .Include(g => g.GameMaster)
                .Include(g => g.Players)
                .ToList()
                .Where(g => g.GameTime <= targetDate);

            foreach(var game in games)
            {
                if(game.ActiveCard != null)
                {
                    game.ActiveCard = null;
                    _context.Update(game);
                    _context.SaveChanges();
                }

                foreach (Card card in game.Cards)
                    _context.RemoveRange(card.Votes);

                _context.Cards.RemoveRange(game.Cards);
                _context.Players.Remove(game.GameMaster);
                _context.Players.RemoveRange(game.Players);
                _context.Games.Remove(game);

                _context.SaveChanges();
                count++;
            }

            // Return count of deleted objects
            return count.ToString();
        }
    }
}
