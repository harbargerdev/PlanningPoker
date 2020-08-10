using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Utilities;
using PlanningPoker.Website.Data;
using PlanningPoker.Website.Models;

namespace PlanningPoker.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGameUtility _gameUtility;
        private readonly IEmailUtility _emailUtility;
        private readonly GameContext _gameContext;

        public HomeController(ILogger<HomeController> logger, IGameUtility gameUtility, IEmailUtility emailUtility, GameContext gameContext)
        {
            _logger = logger;
            _gameUtility = gameUtility;
            _emailUtility = emailUtility;
            _gameContext = gameContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Game Master Methods

        
        public IActionResult GameMasterStart([FromQuery] string playerName, [FromQuery] string gameName)
        {
            Player gameMaster = new Player { PlayerId = Guid.NewGuid(), PlayerName = playerName, PlayerType = PlayerType.GameMaster };
            Game newGame = _gameUtility.InitializeGameFromGameMaster(gameMaster, gameName);

            _gameContext.Add(gameMaster);
            _gameContext.Add(newGame);
            _gameContext.SaveChanges();
            
            ViewBag.Game = newGame;
            ViewBag.Player = gameMaster;
            return View();
        }


        public IActionResult GameMasterZone([FromQuery] Guid gameId, [FromQuery] Guid cardId, [FromQuery] string cardNumber, [FromQuery] string cardSource)
        {
            var game = _gameContext.Games.Include(g => g.Cards).FirstOrDefault(g => g.GameId == gameId);
            Card card;
            if (cardId == Guid.Empty && game != null)
            {
                return GameMasterNewCard(gameId);
            }
            else
            {
                card = _gameContext.Cards.Include(c => c.Votes).ThenInclude(v => v.Player).FirstOrDefault(c => c.CardId == cardId);
                card.CardNumber = cardNumber != null ? cardNumber : string.Empty;
                card.CardSource = cardSource != null ? cardSource : string.Empty;
                game.ActiveCard = card;
                _gameContext.Update(game);
                _gameContext.Update(card);
            }
            _gameContext.SaveChanges();

            ViewBag.Game = game;
            ViewBag.Card = card;

            return View();
        }

        public IActionResult GameMasterFinalizeVoting([FromQuery] Guid gameId, [FromQuery] Guid cardId)
        {
            var game = _gameContext.Games.Include(g => g.Cards).FirstOrDefault(g => g.GameId == gameId);
            var card = _gameContext.Cards.Include(c => c.Votes).ThenInclude(v => v.Player).FirstOrDefault(c => c.CardId == cardId);

            if (card != null && game != null)
            {
                card.IsLocked = true;
                card.IsFinished = true;
                game.ActiveCard = null;

                if (card.Votes.Where(v => v.Player.PlayerType == PlayerType.Developer).Any())
                    card.DeveloperSize = card.Votes.Where(v => v.Player.PlayerType == PlayerType.Developer).Max(v => v.Score);
                
                if (card.Votes.Where(v => v.Player.PlayerType == PlayerType.Tester).Any())
                    card.TestingSize = card.Votes.Where(v => v.Player.PlayerType == PlayerType.Tester).Max(v => v.Score);
                
                card.StorySize = card.Votes.Max(v => v.Score);

                _gameContext.Update(card);
                _gameContext.Update(game);

                _gameContext.SaveChanges();
            }

            ViewBag.Game = game;
            ViewBag.Card = card;

            return View("GameMasterZone");
        }

        public IActionResult GameMasterNewCard([FromQuery] Guid gameId)
        {
            var game = _gameContext.Games.FirstOrDefault(g => g.GameId == gameId);
            Card card = new Card();

            if (game != null)
            {
                if (game.Cards == null)
                    game.Cards = new List<Card>();

                card = _gameUtility.InitializeCardForGame();
                game.Cards.Add(card);
                game.ActiveCard = card;

                _gameContext.Cards.Add(card);
                _gameContext.Update(game);
                _gameContext.SaveChanges();
            }

            ViewBag.Game = game;
            ViewBag.Card = card;

            return View("GameMasterZone");
        }

        public IActionResult GameMasterStartEmail([FromQuery] Guid playerId, [FromQuery] Guid gameId)
        {
            var player = _gameContext.Players.FirstOrDefault(p => p.PlayerId == playerId);
            var game = _gameContext.Games.FirstOrDefault(g => g.GameId == gameId);

            ViewBag.Game = game;
            ViewBag.Player = player;

            return View();
        }

        public IActionResult SendGmStartEmail([FromQuery] string emailAddress, [FromQuery] string playerName, [FromQuery] Guid gameId)
        {
            var game = _gameContext.Games.FirstOrDefault(g => g.GameId == gameId);
            _emailUtility.SendGameStartLinkEmail(emailAddress, playerName, game.GameName, gameId);

            return View("ThankYou");
        }

        public IActionResult GameMasterFinishEmail([FromQuery] Guid gameId)
        {
            var game = _gameContext.Games
                .Include(g => g.Cards)
                .Include(g => g.GameMaster)
                .Include(g => g.Players)
                .FirstOrDefault(g => g.GameId == gameId);

            ViewBag.Game = game;

            return View();
        }

        public IActionResult SendGmSummaryEmail([FromQuery] string emailAddress, [FromQuery] Guid gameId)
        {
            var game = _gameContext.Games
                .Include(g => g.Cards)
                .Include(g => g.GameMaster)
                .Include(g => g.Players)
                .FirstOrDefault(g => g.GameId == gameId);

            _emailUtility.SendGameSummaryEmail(emailAddress, game);

            return View("ThankYou");
        }

        #endregion

        #region Player Methods

        public IActionResult PlayerStart([FromQuery] Guid gameId)
        {
            ViewBag.GameId = gameId;
            return View();
        }

        public IActionResult PlayerCreation([FromQuery] string playerName, [FromQuery] string role, [FromQuery] Guid gameId)
        {
            var game = _gameContext.Games.Include(g => g.Players).FirstOrDefault(g => g.GameId == gameId);
            var player = _gameUtility.InitializePlayer(playerName, role);

            if (game.Players == null)
                game.Players = new List<Player>();

            if (!game.Players.Contains(player))
            {
                game.Players.Add(player);
                _gameContext.Players.Add(player);
                _gameContext.Update(game);
                _gameContext.SaveChanges();
            }

            ViewBag.VotingComplete = false;
            ViewBag.Player = player;
            ViewBag.Game = game;

            return View("PlayerZone");
        }

        public IActionResult PlayerZone([FromQuery] Guid gameId, [FromQuery] Guid playerId, [FromQuery] int size)
        {
            var game = _gameContext.Games.Include(g => g.ActiveCard).Include(p => p.Players).FirstOrDefault(g => g.GameId == gameId);
            var player = _gameContext.Players.FirstOrDefault(p => p.PlayerId == playerId);

            if (game?.ActiveCard != null && !game.ActiveCard.IsFinished)
            {
                HandlePlayerVote(game.ActiveCard, player, size);

                // Get latest version from DB
                game = _gameContext.Games.FirstOrDefault(g => g.GameId == game.GameId);
            }

            ViewBag.VotingComplete = game.ActiveCard?.Votes.Count >= game?.Players.Count();
            ViewBag.Player = player;
            ViewBag.Game = game;

            return View();
        }

        private void HandlePlayerVote(Card card, Player player, int size)
        {
            // This may be dangerous, but the best I can come up with ...
            while (card.IsLocked)
            {
                Thread.Sleep(1000);
                card = _gameContext.Cards.FirstOrDefault(c => c.CardId == card.CardId);
            }

            // Lock the card on the db
            card.IsLocked = true;
            _gameContext.Update(card);
            _gameContext.SaveChanges();

            card = _gameContext.Cards.Include(c => c.Votes).FirstOrDefault(c => c.CardId == card.CardId);

            var vote = card.Votes.FirstOrDefault(v => v.Player.PlayerId == player.PlayerId);

            if (vote == null)
            {
                vote = new Vote { Card = card, Player = player, Score = size, VoteId = Guid.NewGuid() };
                card.Votes.Add(vote);
                _gameContext.Add(vote);
            }
            else
            {
                vote.Score = size;
                _gameContext.Update(vote);
            }
            
            // Release the lock and save the card
            card.IsLocked = false;
            _gameContext.Update(card);
            _gameContext.SaveChanges();
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
