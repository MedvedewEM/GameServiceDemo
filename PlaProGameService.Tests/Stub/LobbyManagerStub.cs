namespace PlaProGameService.Tests.Stub
{
    using Logic.Steam;

    public class LobbyManagerStub: ILobbyManager
    {
        private readonly SteamProfilesResult.Profile playerProfile;

        public LobbyManagerStub()
        {
        }

        public LobbyManagerStub(string gameId, string lobbySteamId)
        {
            playerProfile = new SteamProfilesResult.Profile
            {
                gameid = gameId,
                lobbysteamid = lobbySteamId
            };
        }

        public SteamProfilesResult.Profile GetPlayerSummaries(string proSteamId)
        {
            return playerProfile;
        }
    }
}
