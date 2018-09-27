namespace PlaProGameService.DataContext
{
    using MongoDB.Bson;
    using PlaProGameService.Enum;
    using PlaProGameService.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDataContext
    {
        List<BotModel> GetBots();
        Task UpdateStateBot(ObjectId id, EBotState newState);
    }
}
