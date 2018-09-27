namespace PlaProGameService.Logic.Steam
{
    public interface ILobbyManager
    {
        SteamProfilesResult.Profile GetPlayerSummaries(string proSteamId);
    }
}
