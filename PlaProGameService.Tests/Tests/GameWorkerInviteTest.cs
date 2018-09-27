namespace PlaProGameService.Tests.Tests
{
    using NUnit.Framework;
    using Queries;
    using Enum;
    using Logging;
    using Logic;
    using Mock;
    using Stub;
    using DataContext;

    [TestFixture]
    public class GameWorkerInviteTest
    {
        private MemoryDataContext dataContext;
        private BotRepository botRepository;
        private ILogger logger;
        private SteamClientMock steamClient;
        private GameWorker gameWorker;

        [SetUp]
        public void SetupData()
        {
            dataContext = new MemoryDataContext();
            dataContext.AddBotWith("plaprobot", EBotState.Worked);

            botRepository = new BotRepository(dataContext);
            logger = new LoggerStub();
            steamClient = new SteamClientMock(logger);
            gameWorker = new GameWorker(botRepository, logger, steamClient);
        }

        [Test]
        public void ReturnNoFreeBotsResultWhenAllBotsAreWorkedOrCrashed()
        {
            dataContext.AddBotWith("plaprobot2", EBotState.Crashed);
            var query = new InviteQuery { UserIds = new ulong[] { 123 } };

            var result = gameWorker.Invite(query);

            Assert.AreEqual(EGameWorkerResult.NoFreeBots, result, "Test does not return 'NoFreeBots' result.");
        }

        [Test]
        public void FreeBotIsConnectedWhenExists()
        {
            dataContext.AddBotWith("plaprobot2", EBotState.Free);
            var query = new InviteQuery { UserIds = new ulong[] { 123 } };

            gameWorker.Invite(query);

            Assert.IsTrue(steamClient.IsBotConnected, "Free bot is not connected to steam.");
        }

        [Test]
        public void InviteMethodIsCalledWhenFreeBotIsConnected()
        {
            dataContext.AddBotWith("plaprobot2", EBotState.Free);
            var query = new InviteQuery { UserIds = new ulong[] { 123 } };

            gameWorker.Invite(query);

            CollectionAssert.AreEqual(query.UserIds, steamClient.InvitationRecepient, "Invitations were not sent.");
        }
    }
}
