namespace PlaProGameService.Tests.Mock
{
    using System;
    using System.Collections.Generic;
    using PlaProGameService.Queries;
    using PlaProGameService.Logging;
    using PlaProGameService.Logic;
    using PlaProGameService.Logic.Steam;

    public class SteamClientMock: ISteamClient
    {
        private readonly ILogger logger;

        public bool IsBotConnected { get; set; } = false;
        public List<ulong> InvitationRecepient { get; set; } = new List<ulong> ();

        public SteamClientMock(ILogger logger)
        {
            this.logger = logger;
        }

        public void ConnectBot(BotManager botManager)
        {
            IsBotConnected = true;
        }

        public void LogOffBot()
        {
            throw new NotImplementedException();
        }

        public void DoBotAction(Action<object> methodAction, object parameter)
        {
            methodAction.Invoke(parameter);
        }

        public void InviteToParty(InviteQuery query)
        {
            InvitationRecepient.AddRange(query.UserIds);
        }

        public void SetLeader(ulong proUserId)
        {
            throw new NotImplementedException();
        }

        public bool WaitBotActions()
        {
            return true;
        }
    }
}
