using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Model;

namespace Common.MongoDb
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
