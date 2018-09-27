using MongoDB.Bson;
using MongoDB.Driver;
using PlaProGameService.Enum;
namespace PlaProGameService.DataContext
{
    using PlaProGameService.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataContext: IDataContext
    {
        private readonly IMongoCollection<BotModel> botCollection;

        public DataContext()
        {
            var database = new Database();
            botCollection = database.GetBotCollection();
        }

        public List<BotModel> GetBots()
        {
            return botCollection.AsQueryable().Where(x => true).ToList();
        }

        public async Task UpdateStateBot(ObjectId id, EBotState newState)
        {
            var update = Builders<BotModel>.Update.Set(x => x.State, newState);

            await botCollection.FindOneAndUpdateAsync(x => x.Id == id, update);
        }
    }
}
