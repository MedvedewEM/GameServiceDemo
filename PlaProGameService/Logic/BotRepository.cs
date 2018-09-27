namespace PlaProGameService.Logic
{
    using System.Linq;
    using MongoDB.Bson;
    using DataContext;
    using Enum;
    using Models;

    public class BotRepository
    {
        private readonly IDataContext dataContext;

        public BotRepository(IDataContext botsDataContext)
        {
            dataContext = botsDataContext;
        }

        public string GetBotsStateAsString()
        {
            var bots = dataContext.GetBots()
                .Select(x => new StateBotViewModel {Login = x.Login, State = x.State, LastUsedDate = x.LastUsedDate})
                .ToList();

            return string.Join("\n", bots.Select(x => $"{x.Login}: {x.State} ({x.LastUsedDate:yyyy.MM.dd hh:mm})"));
        }

        public BotModel GetFreeBot()
        {
            return dataContext.GetBots().FirstOrDefault(x => x.State == EBotState.Free);
        }

        public void UpdateStateBot(ObjectId id, EBotState newState)
        {
            dataContext.UpdateStateBot(id, newState);
        }
    }
}
