namespace PlaProGameService.Logic
{
    using Logging;
    using Queries;
    using Steam;
    using Enum;

    public class GameWorker
    {
        private readonly BotRepository botRepository;
        private readonly ILogger logger;
        private readonly ISteamClient steamClient;

        public GameWorker(BotRepository botRepository, ILogger logger, ISteamClient steamClient)
        {
            this.botRepository = botRepository;
            this.logger = logger;
            this.steamClient = steamClient;
        }

        public EGameWorkerResult Invite(InviteQuery query)
        {
            var freeBot = botRepository.GetFreeBot();
            if (freeBot == null)
            {
                logger.Warn($"There are no avaliable bots. Users with ids are not invited: {string.Join(",", query.UserIds)}");
                return EGameWorkerResult.NoFreeBots;
            }
            
            var botManager = new BotManager(botRepository, freeBot);

            steamClient.ConnectBot(botManager);

            steamClient.DoBotAction(x => steamClient.InviteToParty((InviteQuery)x), query);

            var botActionsResult = steamClient.WaitBotActions();

            if (!botActionsResult)
                return Invite(query);

            return EGameWorkerResult.Ok;
        }

        public static string GetProLobbyUrl(ILobbyManager lobbyManager, string proSteamId)
        {
            var profileData = lobbyManager.GetPlayerSummaries(proSteamId);

            var gameId = profileData?.gameid;
            var lobbyId = profileData?.lobbysteamid;

            if (gameId == null || lobbyId == null)
                return null;

            return $"steam://joinlobby/{gameId}/{lobbyId}/{proSteamId}";
        }
    }
}
