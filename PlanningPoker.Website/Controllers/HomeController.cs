using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

            _gameContext.Update(game);
            _gameContext.SaveChanges();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
