namespace PlaProGameService.Tests.Tests
{
    using NUnit.Framework;
    using Logic;
    using Stub;

    [TestFixture]
    public class GameWorkerLobbyTest
    {
        [Test]
        public void ReturnNullWhenGameIdIsNull()
        {
            var lobbyManager = new LobbyManagerStub(null, "1234");
            var lobbyUrl = GameWorker.GetProLobbyUrl(lobbyManager, "123456");

            Assert.IsNull(lobbyUrl, "Lobby url must be null when gameId is null.");
        }

        [Test]
        public void ReturnNullWhenLobbySteamIdIsNull()
        {
            var lobbyManager = new LobbyManagerStub("9876", null);
            var lobbyUrl = GameWorker.GetProLobbyUrl(lobbyManager, "123456");

            Assert.IsNull(lobbyUrl, "Lobby url must be null when lobbySteamId is null.");
        }

        [Test]
        public void ReturnNullWhenProfileDataIsNull()
        {
            var lobbyManager = new LobbyManagerStub();
            var lobbyUrl = GameWorker.GetProLobbyUrl(lobbyManager, "123456");

            Assert.IsNull(lobbyUrl, "Lobby url must be null when profile data is null.");
        }

        [Test]
        public void ReturnCorrectLobbyUrlWhenPlayerHasCorrectProfileData()
        {
            const string gameId = "9876";
            const string lobbyId = "1234";
            const string playerId = "13579";
            var lobbyManager = new LobbyManagerStub(gameId, lobbyId);
            var lobbyUrl = GameWorker.GetProLobbyUrl(lobbyManager, playerId);

            Assert.IsNotNull(lobbyUrl, "LobbyUrl must be not null.");
            StringAssert.IsMatch($"steam://joinlobby/{gameId}/{lobbyId}/{playerId}", lobbyUrl, "Lobby has incorrect format.");
        }
    }
}
