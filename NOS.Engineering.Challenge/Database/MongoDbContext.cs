using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Database
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Content> Contents => _database.GetCollection<Content>("Contents");
    }
}
