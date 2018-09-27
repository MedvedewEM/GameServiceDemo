namespace PlaProGameService.Logic.Steam
{
    using System.Linq;
    using System.Net;
    using Newtonsoft.Json;

    public class LobbyManager: ILobbyManager
    {
        private const string SteamApiSummariesUrl = "https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=BCBD5FC84153A77C6886D43D7C92B56C&steamids=";

        private readonly WebClient webClient;

        public LobbyManager(WebClient webClient)
        {
            this.webClient = webClient;
        }

        public SteamProfilesResult.Profile GetPlayerSummaries(string proSteamId)
        {
            var response = webClient.DownloadString(SteamApiSummariesUrl + proSteamId);
            if (string.IsNullOrEmpty(response))
                return null;

            var result = JsonConvert.DeserializeObject<SteamProfilesResult>(response);
            return result.response.players.FirstOrDefault();
        }
    }
}
