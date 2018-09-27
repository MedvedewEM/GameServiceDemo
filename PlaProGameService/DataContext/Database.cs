namespace PlaProGameService.DataContext
{
    using MongoDB.Driver;
    using PlaProGameService.Models;

    public class Database
    {
        private readonly IMongoDatabase database;

        public Database()
        {
            var client = new MongoClient(Config.Configuration.Server);
            database = client.GetDatabase(Config.Configuration.Database);
        }

        public IMongoCollection<BotModel> GetBotCollection()
        {
            return database.GetCollection<BotModel>("GameServiceBot");
        }
    }
}
