namespace PlaProGameService.Logic.Steam
{
    using System;
    using Queries;

    public interface ISteamClient
    {
        void ConnectBot(BotManager bot);
        void LogOffBot();
        void DoBotAction(Action<object> methodAction, object parameter);
        void InviteToParty(InviteQuery query);
        void SetLeader(ulong proUserId);
        bool WaitBotActions();
    }
}
