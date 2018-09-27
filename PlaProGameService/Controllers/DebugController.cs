namespace PlaProGameService.Controllers
{
    using DataContext;
    using Logging;
    using Logic;
    using Logic.Steam;
    using Enum;
    using Microsoft.AspNetCore.Mvc;
    using Queries;
    using PlaProGameService.Models;

    [Route("api/[controller]")]
    public class DebugController : Controller
    {
        private readonly ILogger logger;

        public DebugController(ILogger logger)
        {
            this.logger = logger;
        }

        [HttpPost("invite")]
        public EGameWorkerResult Invite([FromBody] DebugInviteQuery query)
        {
            var dataContext = new MemoryDataContext();
            dataContext.AddBot(
                new BotModel
                {
                    Login = query.Login,
                    Password = query.Password,
                    State = EBotState.Free
                }
            );

            var botRepository = new BotRepository(dataContext);

            var steamClient = new SteamClient(logger);
            var gameWorker = new GameWorker(botRepository, logger, steamClient);

            var result = gameWorker.Invite(query.InviteSubQuery);

            return result;
        }
    }

    public class DebugInviteQuery
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public InviteQuery InviteSubQuery { get; set; }
    }
}
