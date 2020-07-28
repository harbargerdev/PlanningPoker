using PlanningPoker.Core.Entities;
using System;
using System.Collections.Generic;

namespace PlanningPoker.Core.Utilities
{
    public interface IGameUtility
    {
        /// <summary>
        /// Method Intializes a <see cref="Game"/> based on input parameters and defaults
        /// </summary>
        /// <param name="gameMaster">The Game Master</param>
        /// <param name="title">The Game Title</param>
        /// <returns>A new instance of a <see cref="Game" /></returns>
        Game InitializeGameFromGameMaster(Player gameMaster, string title);
    }

    public class GameUtility : IGameUtility
    {
        /// <inheritdoc />
        public Game InitializeGameFromGameMaster(Player gameMaster, string title)
        {
            return new Game
            {
                GameId = new Guid(),
                GameMaster = gameMaster,
                GameTime = DateTime.Now,
                GameName = title,
                Players = new List<Player>()
            };
        }
    }
}
