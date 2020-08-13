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
        Guid playerId;
        Guid gameId;
        Guid cardId;
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

            playerId = Guid.NewGuid();
            gameId = Guid.NewGuid();
            cardId = Guid.NewGuid();

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

        [Test]
        public void GameMasterZone_ExecutesNewCardMethod()
        {
            // Arrange
            var game = new Game
            {
                GameId = gameId,
                GameName = "Game Name",
                GameTime = DateTime.Now
            };
            var card = new Card
            {
                CardId = cardId,
                StorySize = 0,
                DeveloperSize = 0,
                TestingSize = 0,
                DeveloperVotes = 0,
                TestingVotes = 0,
                CardNumber = string.Empty,
                CardSource = string.Empty,
                IsLocked = false,
                IsFinished = false,
                Votes = new List<Vote>()
            };

            gameContext.Add(game);
            gameContext.SaveChanges();

            gameUtilityMock.Reset();
            gameUtilityMock.Setup(gu => gu.InitializeCardForGame()).Returns(card);

            // Act
            var controller = new HomeController(loggerMock.Object, gameUtilityMock.Object, emailUtilityMock.Object, gameContext);
            var response = controller.GameMasterZone(gameId, Guid.Empty, null, null) as ViewResult;

            // Assert
            gameUtilityMock.Verify();
            Assert.AreEqual(card, response.ViewData["Card"]);
        }

        [Test]
        public void GameMasterZone_ExecuteExistingCard()
        {
            // Arrange
            var game = new Game { GameId = gameId, GameName = "Game Name", GameTime = DateTime.Now };
            var card = new Card
            {
                CardId = cardId,
                StorySize = 0,
                DeveloperSize = 0,
                TestingSize = 0,
                DeveloperVotes = 0,
                TestingVotes = 0,
                CardNumber = string.Empty,
                CardSource = string.Empty,
                IsLocked = false,
                IsFinished = false,
                Votes = new List<Vote>()
            };

            gameContext.Add(game);
            gameContext.Add(card);
            gameContext.SaveChanges();

            // Act
            var controller = new HomeController(loggerMock.Object, gameUtilityMock.Object, emailUtilityMock.Object, gameContext);
            var response = controller.GameMasterZone(gameId, cardId, "Card Number", "Card Source") as ViewResult;

            // Assert
            card.CardNumber = "Card Number";
            card.CardSource = "Card Source";
            game.ActiveCard = card;
            Assert.AreEqual(card, response.ViewData["Card"]);
            Assert.AreEqual(game, response.ViewData["Game"]);
        }

        [Test]
        public void GameMasterFinalizeVoting_NoTestingVotes()
        {
            // Arrange
            var game = new Game { GameId = gameId, GameName = "Game Name", GameTime = DateTime.Now };
            var card = new Card { CardId = cardId };
            
            gameContext.Add(game);
            gameContext.Add(card);
            gameContext.SaveChanges();

            var votes = GenerateDeveloperVotes(3, card, 5);
            card.Votes = votes;

            gameContext.AddRange(votes);
            gameContext.Update(card);
            gameContext.SaveChanges();

            // Act
            var controller = new HomeController(loggerMock.Object, gameUtilityMock.Object, emailUtilityMock.Object, gameContext);
            var response = controller.GameMasterFinalizeVoting(gameId, cardId) as ViewResult;
            var resultedCard = response.ViewData["Card"] as Card;

            // Assert
            Assert.AreEqual(5, resultedCard.DeveloperSize);
            Assert.AreEqual(0, resultedCard.TestingSize);
            Assert.AreEqual(5, resultedCard.StorySize);
        }

        [Test]
        public void GameMasterFinalizeVoting_NoDeveloperVotes()
        {
            // Arrange
            var game = new Game { GameId = gameId, GameName = "Game Name", GameTime = DateTime.Now };
            var card = new Card { CardId = cardId };

            gameContext.Add(game);
            gameContext.Add(card);
            gameContext.SaveChanges();

            var votes = GenerateTesterVotes(3, card, 5);
            card.Votes = votes;

            gameContext.AddRange(votes);
            gameContext.Update(card);
            gameContext.SaveChanges();

            // Act
            var controller = new HomeController(loggerMock.Object, gameUtilityMock.Object, emailUtilityMock.Object, gameContext);
            var response = controller.GameMasterFinalizeVoting(gameId, cardId) as ViewResult;
            var resultedCard = response.ViewData["Card"] as Card;

            // Assert
            Assert.AreEqual(0, resultedCard.DeveloperSize);
            Assert.AreEqual(5, resultedCard.TestingSize);
            Assert.AreEqual(5, resultedCard.StorySize);
        }

        [Test]
        public void GameMasterFinalizeVoting_DeveloperAndTesterVotes()
        {
            // Arrange
            var game = new Game { GameId = gameId, GameName = "Game Name", GameTime = DateTime.Now };
            var card = new Card { CardId = cardId, Votes = new List<Vote>() };

            gameContext.Add(game);
            gameContext.Add(card);
            gameContext.SaveChanges();

            var devVotes = GenerateDeveloperVotes(3, card, 8);
            var testVotes = GenerateTesterVotes(3, card, 5);
            card.Votes.AddRange(devVotes);
            card.Votes.AddRange(testVotes);

            gameContext.AddRange(devVotes);
            gameContext.AddRange(testVotes);
            gameContext.Update(card);
            gameContext.SaveChanges();

            // Act
            var controller = new HomeController(loggerMock.Object, gameUtilityMock.Object, emailUtilityMock.Object, gameContext);
            var response = controller.GameMasterFinalizeVoting(gameId, cardId) as ViewResult;
            var resultedCard = response.ViewData["Card"] as Card;

            // Assert
            Assert.AreEqual(8, resultedCard.DeveloperSize);
            Assert.AreEqual(5, resultedCard.TestingSize);
            Assert.AreEqual(8, resultedCard.StorySize);
        }

        [Test]
        public void GameMasterRevote_ResetsVotes()
        {
            // Arrange
            var game = new Game { GameId = gameId, GameName = "Game Name", GameTime = DateTime.Now };
            var card = new Card { CardId = cardId, Votes = new List<Vote>(), DeveloperSize = 5, TestingSize = 5, StorySize = 5 };

            gameContext.Add(game);
            gameContext.Add(card);
            gameContext.SaveChanges();
            
            var devVotes = GenerateDeveloperVotes(3, card, 5);
            var testVotes = GenerateTesterVotes(3, card, 5);
            card.Votes.AddRange(devVotes);
            card.Votes.AddRange(testVotes);

            gameContext.AddRange(devVotes);
            gameContext.AddRange(testVotes);
            gameContext.Update(card);
            gameContext.SaveChanges();

            // Act
            var controller = new HomeController(loggerMock.Object, gameUtilityMock.Object, emailUtilityMock.Object, gameContext);
            var response = controller.GameMasterRevote(gameId, cardId) as ViewResult;

            var resultingCard = response.ViewData["Card"] as Card;

            // Assert
            Assert.AreEqual(0, resultingCard.DeveloperSize);
            Assert.AreEqual(0, resultingCard.TestingSize);
            Assert.AreEqual(0, resultingCard.StorySize);
            Assert.AreEqual(0, resultingCard.Votes.Count);
        }

        [Test]
        public void SendGmStartEmail_CallsEmailUtility()
        {
            // Arrange
            var email = "something@email.com";
            var game = new Game { GameId = gameId, GameName = "Game Name", GameTime = DateTime.Now };
            var player = new Player { PlayerId = playerId, PlayerName = "Game Master", PlayerType = PlayerType.GameMaster };

            gameContext.Add(game);
            gameContext.Add(player);
            gameContext.SaveChanges();

            emailUtilityMock.Reset();
            emailUtilityMock.Setup(eu => eu.SendGameStartLinkEmail(email, player.PlayerName, game.GameName, playerId, gameId));

            // Act
            var controller = new HomeController(loggerMock.Object, gameUtilityMock.Object, emailUtilityMock.Object, gameContext);
            var response = controller.SendGmStartEmail(email, playerId, gameId) as ViewResult;

            // Assert
            emailUtilityMock.Verify();
            Assert.AreEqual("ThankYou", response.ViewName);
        }

        [Test]
        public void GameMasterFinishEmail_PopulatesObjects()
        {
            // Arrange
            var cards = new List<Card>();
            for (int i = 0; i < 3; i++)
            {
                cards.Add(new Card { CardId = Guid.NewGuid() });
            }
            gameContext.AddRange(cards);

            var players = new List<Player>();
            for (int i=0; i < 3; i++)
            {
                players.Add(new Player { PlayerId = Guid.NewGuid(), PlayerType = PlayerType.Developer });
            }
            gameContext.AddRange(players);

            var player = new Player { PlayerId = playerId, PlayerName = "Game Master", PlayerType = PlayerType.GameMaster };
            gameContext.Add(player);

            var game = new Game
            {
                GameId = gameId,
                GameName = "Game Name",
                GameTime = DateTime.Now,
                GameMaster = player,
                Players = players,
                Cards = cards
            };
            gameContext.Add(game);
            gameContext.SaveChanges();

            // Act
            var controller = new HomeController(loggerMock.Object, gameUtilityMock.Object, emailUtilityMock.Object, gameContext);
            var response = controller.GameMasterFinishEmail(gameId) as ViewResult;

            var output = response.ViewData["Game"] as Game;

            // Assert
            Assert.NotNull(output.Cards);
            Assert.NotNull(output.GameMaster);
            Assert.NotNull(output.Players);
        }

        [Test]
        public void SendGmSummaryEmail_CallEmailUtilityAndReturnsView()
        {
            // Arrange
            var email = "something@email.com";
            var cards = new List<Card>();
            for (int i = 0; i < 3; i++)
            {
                cards.Add(new Card { CardId = Guid.NewGuid() });
            }
            gameContext.AddRange(cards);

            var players = new List<Player>();
            for (int i = 0; i < 3; i++)
            {
                players.Add(new Player { PlayerId = Guid.NewGuid(), PlayerType = PlayerType.Developer });
            }
            gameContext.AddRange(players);

            var player = new Player { PlayerId = playerId, PlayerName = "Game Master", PlayerType = PlayerType.GameMaster };
            gameContext.Add(player);

            var game = new Game
            {
                GameId = gameId,
                GameName = "Game Name",
                GameTime = DateTime.Now,
                GameMaster = player,
                Players = players,
                Cards = cards
            };
            gameContext.Add(game);
            gameContext.SaveChanges();

            emailUtilityMock.Reset();
            emailUtilityMock.Setup(eu => eu.SendGameSummaryEmail(email, game));

            // Act
            var controller = new HomeController(loggerMock.Object, gameUtilityMock.Object, emailUtilityMock.Object, gameContext);
            var response = controller.SendGmSummaryEmail(email, gameId) as ViewResult;

            // Assert
            emailUtilityMock.Verify();
            Assert.AreEqual("ThankYou", response.ViewName);
        }

        private List<Vote> GenerateDeveloperVotes(int count, Card card, int size)
        {
            List<Vote> votes = new List<Vote>();

            for (int i = 0; i < count; i++)
            {
                var vote = new Vote
                {
                    Card = card,
                    Player = new Player { PlayerId = Guid.NewGuid(), PlayerName = "Developer", PlayerType = PlayerType.Developer },
                    Score = size,
                    VoteId = Guid.NewGuid()
                };
                votes.Add(vote);
            }

            return votes;
        }

        private List<Vote> GenerateTesterVotes(int count, Card card, int size)
        {
            List<Vote> votes = new List<Vote>();

            for (int i = 0; i < count; i++)
            {
                var vote = new Vote
                {
                    Card = card,
                    Player = new Player { PlayerId = Guid.NewGuid(), PlayerName = "Tester", PlayerType = PlayerType.Tester },
                    Score = size,
                    VoteId = Guid.NewGuid()
                };
                votes.Add(vote);
            }

            return votes;
        }
    }
}
