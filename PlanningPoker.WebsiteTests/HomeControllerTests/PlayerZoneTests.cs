using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Utilities;
using PlanningPoker.Website.Controllers;
using PlanningPoker.Website.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlanningPoker.WebsiteTests.HomeControllerTests
{
    [TestFixture]
    public class PlayerZoneTests
    {
        Mock<ILogger<HomeController>> _loggerMock;
        Mock<IGameUtility> _gameUtilityMock;
        Mock<IEmailUtility> _emailUtilityMock;
        GameContext _gameContext;

        Guid gameId;
        Guid playerId;

        [SetUp]
        public void SetupMocks()
        {
            // New up mocks
            _loggerMock = new Mock<ILogger<HomeController>>();
            _gameUtilityMock = new Mock<IGameUtility>();
            _emailUtilityMock = new Mock<IEmailUtility>();

            gameId = Guid.NewGuid();
            playerId = Guid.NewGuid();

            // Setup GameContext
            var options = new DbContextOptionsBuilder<GameContext>()
                                .UseInMemoryDatabase(databaseName: "InMemoryPlanningPokerDb")
                                .Options;
            _gameContext = new InMemoryGameContext(options);
        }

        [Test]
        public void PlayerCreation_NewPlayer()
        {
            // Arrange
            string playerName = "Developer";
            string playerType = "developer";
            var game = new Game { GameId = gameId };
            _gameContext.Add(game);
            _gameContext.SaveChanges();

            var player = new Player { PlayerId = playerId, PlayerName = playerName, PlayerType = PlayerType.Developer };

            _gameUtilityMock.Reset();
            _gameUtilityMock.Setup(gu => gu.InitializePlayer(playerName, playerType)).Returns(player);

            // Act
            var controller = new HomeController(_loggerMock.Object, _gameUtilityMock.Object, _emailUtilityMock.Object, _gameContext);
            var response = controller.PlayerCreation(playerName, playerType, gameId) as ViewResult;

            var outGame = response.ViewData["Game"] as Game;
            var outPlayer = response.ViewData["Player"] as Player;

            // Assert
            _gameUtilityMock.Verify();
            Assert.AreEqual("PlayerZone", response.ViewName);
            Assert.Contains(player, outGame.Players);
        }

        [Test]
        public void PlayerZone_HandleNewVoteSingleVoter()
        {
            // Arrange
            var player = new Player { PlayerId = playerId, PlayerName = "Developer", PlayerType = PlayerType.Developer };
            _gameContext.Add(player);
            _gameContext.SaveChanges();

            var card = new Card { CardId = Guid.NewGuid(), Votes = new List<Vote>() };
            _gameContext.Add(card);
            _gameContext.SaveChanges();

            var game = new Game
            {
                GameId = gameId,
                Players = new List<Player> { player },
                Cards = new List<Card> { card },
                ActiveCard = card
            };
            _gameContext.Add(game);
            _gameContext.SaveChanges();

            // Act
            var controller = new HomeController(_loggerMock.Object, _gameUtilityMock.Object, _emailUtilityMock.Object, _gameContext);
            var response = controller.PlayerZone(gameId, playerId, 5) as ViewResult;

            var outGame = response.ViewData["Game"] as Game;

            // Assert
            Assert.AreEqual(5, outGame.ActiveCard.Votes[0].Score);
        }

        [Test]
        public void PlayerZone_HandleExistingVoteSingleVoter()
        {
            // Arrange
            var player = new Player { PlayerId = playerId, PlayerName = "Developer", PlayerType = PlayerType.Developer };
            _gameContext.Add(player);

            var card = new Card { CardId = Guid.Empty, Votes = new List<Vote>() };
            _gameContext.Add(card);

            _gameContext.SaveChanges();

            var vote = new Vote { Card = card, Player = player, Score = 3, VoteId = Guid.Empty };
            card.Votes.Add(vote);
            _gameContext.Add(vote);
            _gameContext.Update(card);
            _gameContext.SaveChanges();

            var game = new Game
            {
                GameId = gameId,
                Players = new List<Player> { player },
                Cards = new List<Card> { card },
                ActiveCard = card
            };
            _gameContext.Add(game);
            _gameContext.SaveChanges();

            // Act
            var controller = new HomeController(_loggerMock.Object, _gameUtilityMock.Object, _emailUtilityMock.Object, _gameContext);
            var response = controller.PlayerZone(gameId, playerId, 5) as ViewResult;

            var outGame = response.ViewData["Game"] as Game;

            // Assert
            Assert.AreEqual(5, outGame.ActiveCard.Votes[0].Score);
        }
    }
}
