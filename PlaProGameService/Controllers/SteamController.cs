namespace PlaProGameService.Controllers
{
    using DataContext;
    using Logging;
    using Logic;
    using Logic.Steam;
    using Enum;
    using Microsoft.AspNetCore.Mvc;
    using Queries;
    using System.Net;

    [Route("api/[controller]")]
    public class SteamController : Controller
    {
        private readonly ILogger logger;
        private readonly IDataContext botDataContext;

        public SteamController(IDataContext dataContext, ILogger gameLogger)
        {
            botDataContext = dataContext;
            logger = gameLogger;
        }

        [HttpGet("bots")]
        public string Bots()
        {
            var botRepository = new BotRepository(botDataContext);
            logger.Info("Bots is obtained!");

            var result = botRepository.GetBotsStateAsString();

            return result;
        }

        [HttpPost("invite")]
        public EGameWorkerResult Invite([FromBody] InviteQuery query)
        {
            if (query.UserIds == null || query.UserIds.Length < 1)
                return EGameWorkerResult.InvalidQuery;

            var botRepository = new BotRepository(botDataContext);
            var steamClient = new SteamClient(logger);
            var gameWorker = new GameWorker(botRepository, logger, steamClient);

            var result = gameWorker.Invite(query);

            return result;
        }

        [HttpGet("prolobbyurl")]
        public string ProLobbyUrl(string proId)
        {
            if (string.IsNullOrEmpty(proId))
                return null;

            var webClient = new WebClient();
            var lobbyManager = new LobbyManager(webClient);

            var result = GameWorker.GetProLobbyUrl(lobbyManager, proId);

            return result;
        }
    }
}
