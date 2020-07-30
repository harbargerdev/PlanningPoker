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

        /// <summary>
        /// Method Intializes a <see cref="Card"/> based on input paramters and defaults
        /// </summary>
        /// <param name="cardNumber">The Story Card Number</param>
        /// <param name="cardSource">The Story Card Source</param>
        /// <returns>A new instance of a <see cref="Card" /></returns>
        Card IntializeCardForGame(string cardNumber, string cardSource);

        /// <summary>
        /// Method Initilaizes a <see cref="Player"/> based on input paramters and defaults
        /// </summary>
        /// <param name="playerName">The Player Name</param>
        /// <param name="playerType">The Player Type</param>
        /// <returns>A new instance of a <see cref="Player" /></returns>
        Player InitializePlayer(string playerName, string playerType);
    }

    public class GameUtility : IGameUtility
    {
        /// <inheritdoc />
        public Game InitializeGameFromGameMaster(Player gameMaster, string title)
        {
            return new Game
            {
                GameId = Guid.NewGuid(),
                GameMaster = gameMaster,
                GameTime = DateTime.Now,
                GameName = title,
                Players = new List<Player>(),
                Cards = new List<Card>()
            };
        }

        /// <inheritdoc />
        public Card IntializeCardForGame(string cardNumber, string cardSource)
        {
            return new Card
            {
                CardId = Guid.NewGuid(),
                StorySize = 0,
                DeveloperSize = 0,
                TestingSize = 0,
                DeveloperVotes = 0,
                TestingVotes = 0,
                CardNumber = cardNumber,
                CardSource = cardSource,
                IsLocked = false
            };
        }

        /// <inheritdoc />
        public Player InitializePlayer(string playerName, string playerType)
        {
            PlayerType type = playerType.Equals("Developer") ? PlayerType.Developer : PlayerType.Tester;

            return new Player
            {
                PlayerId = Guid.NewGuid(),
                PlayerName = playerName,
                PlayerType = type
            };
        }
    }
}
