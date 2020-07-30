using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        private readonly GameContext _gameContext;

        public HomeController(ILogger<HomeController> logger, IGameUtility gameUtility, GameContext gameContext)
        {
            _logger = logger;
            _gameUtility = gameUtility;
            _gameContext = gameContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GameMasterStart([FromQuery] string playerName, [FromQuery] string gameName)
        {
            Player gameMaster = new Player { PlayerId = Guid.NewGuid(), PlayerName = playerName, PlayerType = PlayerType.GameMaster };
            Game newGame = _gameUtility.InitializeGameFromGameMaster(gameMaster, gameName);

            _gameContext.Add(gameMaster);
            _gameContext.Add(newGame);
            _gameContext.SaveChanges();
            
            ViewBag.Game = newGame;
            return View();
        }

        public IActionResult PlayerStart([FromQuery] Guid gameId)
        {
            ViewBag.GameId = gameId;
            return View();
        }

        public IActionResult PlayerZone([FromQuery] string playerName, [FromQuery] string role, [FromQuery] Guid gameId)
        {
            var game = _gameContext.Games.FirstOrDefault(g => g.GameId == gameId);
            var player = _gameUtility.InitializePlayer(playerName, role);

            if (game.Players == null)
                game.Players = new List<Player>();
            
            game.Players.Add(player);
            _gameContext.Players.Add(player);
            _gameContext.Update(game);
            _gameContext.SaveChanges();

            return View();
        }

        public IActionResult GameMasterZone([FromQuery] Guid gameId, [FromQuery] Guid cardId, [FromQuery] string cardNumber,
            [FromQuery] string cardSource, [FromQuery] string action)
        {
            Card card;
            if (cardId == Guid.Empty)
            {
                card = _gameUtility.IntializeCardForGame(string.Empty, string.Empty);
                _gameContext.Cards.Add(card);
            }   
            else
            {
                card = _gameContext.Cards.FirstOrDefault(c => c.CardId == cardId);
                card.CardNumber = cardNumber != null ? cardNumber : string.Empty;
                card.CardSource = cardSource != null ? cardSource : string.Empty;
                card.IsLocked = !action.Equals("Submit");
                _gameContext.Update(card);
            }
            _gameContext.SaveChanges();

            var game = _gameContext.Games.FirstOrDefault(g => g.GameId == gameId);

            ViewBag.Game = game;
            ViewBag.Card = card;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
