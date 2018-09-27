namespace PlaProGameService.DataContext
{
    using MongoDB.Bson;
    using PlaProGameService.Enum;
    using PlaProGameService.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class MemoryDataContext : IDataContext
    {
        private List<BotModel> bots = new List<BotModel>();

        public void AddBot(BotModel bot)
        {
            bots.Add(bot);
        }

        public void AddBotWith(string login, EBotState state)
        {
            var bot = new BotModel
            {
                Login = login,
                State = state,
            };

            AddBot(bot);
        }

        public BotModel GetBotByLogin(string login)
        {
            return bots.FirstOrDefault(x => x.Login == login);
        }

        public List<BotModel> GetBots()
        {
            return bots;
        }

        public Task UpdateStateBot(ObjectId id, EBotState newState)
        {
            var bot = bots.FirstOrDefault(x => x.Id == id);
            if (bot == null)
                return Task.FromResult(false);

            bot.State = newState;

            return Task.FromResult(true);
        }
    }
}
