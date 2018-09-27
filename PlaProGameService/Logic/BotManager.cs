namespace PlaProGameService.Logic
{
    using MongoDB.Bson;
    using Enum;
    using Models;
    using SteamKit2;

    public class BotManager
    {
        private readonly BotRepository botRepository;
        private readonly BotModel bot;

        public BotManager(BotRepository botRepository, BotModel botModel)
        {
            this.botRepository = botRepository;
            this.bot = botModel;
        }

        public string GetBotLogin => bot.Login;

        public SteamUser.LogOnDetails GetLogOnParameters =>
            new SteamUser.LogOnDetails {Username = bot.Login, Password = bot.Password, LoginID = 1};

        public void UpdateBotState(EBotState newState)
        {
            if (bot.State == EBotState.Crashed)
                return;

            bot.State = newState;
            botRepository.UpdateStateBot(bot.Id, newState);
        }
    }
}
