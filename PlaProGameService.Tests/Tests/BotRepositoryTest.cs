namespace PlaProGameService.Tests.Tests
{
    using Enum;
    using Logic;
    using MongoDB.Bson;
    using NUnit.Framework;
    using PlaProGameService.DataContext;
    using PlaProGameService.Models;

    [TestFixture]
    public class BotRepositoryTest
    {
        [Test]
        public void ReturnNullWhenAllBotsAreWorkedOrCrashed()
        {
            var botDataContext = new MemoryDataContext();
            var bot1 = new BotModel
            {
                Login = "plaprobot",
                State = EBotState.Worked,
            };
            botDataContext.AddBot(bot1);

            var bot2 = new BotModel
            {
                Login = "plaprobot2",
                State = EBotState.Crashed,
            };
            botDataContext.AddBot(bot2);

            var botRepository = new BotRepository(botDataContext);

            var resultBot = botRepository.GetFreeBot();

            Assert.IsNull(resultBot, "There are free bots in this test.");
        }

        [Test]
        public void LoginIsCorrectWhenFreeBotIsExisted()
        {
            var botDataContext = new MemoryDataContext();
            const string botLogin = "freeBot";
            var bot = new BotModel
            {
                Login = botLogin,
                State = EBotState.Free,
            };
            botDataContext.AddBot(bot);
            var botRepository = new BotRepository(botDataContext);

            var resultBot = botRepository.GetFreeBot();

            Assert.AreEqual(botLogin, resultBot.Login, "Not correct bot's login.");
        }

        [Test]
        public void UpdateBotStateFromWorkedToCrashed()
        {
            var botDataContext = new MemoryDataContext();
            var bot = new BotModel
            {
                Id = new ObjectId("597dd372ab6fc118d85cc08d"),
                Login = "plaprobot",
                State = EBotState.Worked,
            };
            botDataContext.AddBot(bot);
            var botsHandler = new BotRepository(botDataContext);

            botsHandler.UpdateStateBot(bot.Id, EBotState.Crashed);

            var updatedBot = botDataContext.GetBotByLogin("plaprobot");
            Assert.AreEqual(EBotState.Crashed, updatedBot.State, "Bot's state is not updated.");
        }
    }
}
