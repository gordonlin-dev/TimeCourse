using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public interface IMongoDBService
    {
        public IMongoDatabase GetDatabase();
    }

    public class MongoDBService : IMongoDBService
    {
        private readonly IMongoDatabase _database;
        public MongoDBService(IOptions<MongoDbSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            _database = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        }

        public IMongoDatabase GetDatabase()
        {
            return _database;
        }
    }
}
