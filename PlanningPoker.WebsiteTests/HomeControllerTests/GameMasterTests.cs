using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit;
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
    public class GameMasterTests
    {
        Mock<ILogger<HomeController>> loggerMock = null;
        Mock<IGameUtility> gameUtilityMock = null;
        Mock<IEmailUtility> emailUtilityMock = null;
        GameContext gameContext = null;

        [SetUp]
        public void SetupMocks()
        {
            loggerMock = new Mock<ILogger<HomeController>>();
            gameUtilityMock = new Mock<IGameUtility>();
            emailUtilityMock = new Mock<IEmailUtility>();

            // DB Context Setup
            var options = new DbContextOptionsBuilder<GameContext>()
                                .UseInMemoryDatabase(databaseName: "InMemoryPlanningPoker")
                                .Options;
            gameContext = new GameContext(options);
        }

        [Test]
        public void GameMasterStart_ReturnsViewWithGame()
        {
            // Arrange
            string playerName = "Test Player";
            string gameName = "Test Game";
            var _gameReturned = new Game
            {
                GameId = Guid.NewGuid(),
                GameTime = DateTime.Now,
                GameName = gameName,
                Players = new List<Player>(),
                Cards = new List<Card>()
            };

            // Game Utility Setup
            gameUtilityMock.Reset();
            gameUtilityMock.Setup(gu => gu.InitializeGameFromGameMaster(It.IsAny<Player>(), gameName)).Returns(_gameReturned);

            // Act
            var controller = new HomeController(loggerMock.Object, gameUtilityMock.Object, emailUtilityMock.Object, gameContext);
            var response = controller.GameMasterStart(playerName, gameName) as ViewResult;
            
            // Assert
            gameUtilityMock.Verify();
            Assert.AreEqual(_gameReturned, response.ViewData["Game"]);
        }

        [Test]
        public void GameMasterReturn_ReturnsPlayerAndGameView()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            var player = new Player { PlayerId = playerId, PlayerName = "Game Master", PlayerType = PlayerType.GameMaster };
            var game = new Game
            {
                GameId = gameId,
                GameName = "Game Name",
                GameTime = DateTime.Now,
                GameMaster = player
            };

            // Insert to InMemory DB
            gameContext.Add(player);
            gameContext.Add(game);
            gameContext.SaveChanges();

            // Act
            var controller = new HomeController(loggerMock.Object, gameUtilityMock.Object, emailUtilityMock.Object, gameContext);
            var response = controller.GameMasterReturn(playerId, gameId) as ViewResult;

            // Assert
            Assert.AreEqual("GameMasterStart", response.ViewName);
            Assert.AreEqual(player, response.ViewData["Player"]);
            Assert.AreEqual(game, response.ViewData["Game"]);
        }
    }
}
