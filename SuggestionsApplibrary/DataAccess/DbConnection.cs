
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SuggestionsApplibrary.Models;

namespace SuggestionsApplibrary.DataAccess
{
    public class DbConnection :IDbConnection
    {
        private readonly IConfiguration _config;
        private readonly IMongoDatabase _db;
        private string _connectionId = "MongoDB";

        public string DbName { get; private set; }

        public string categoryCollectionName { get; private set; } = "category";

        public string StatusCollectionName { get; private set; } = "statuses";

        public string UserCollectionName { get; private set; } = "user";

        public string SuggestionCollectionName { get; private set; } = "suggestions";

        public MongoClient Client { get; private set; }

        public IMongoCollection<CategoryModel> CategoryCollection { get; private set; }

        public IMongoCollection<StatusModel> StatusCollection { get; private set; }

        public IMongoCollection<UserModel> UserCollection { get; private set; }

        public IMongoCollection<SuggestionModel> SuggestioinCollection { get; private set; }


        public DbConnection(IConfiguration config)
        {
            _config = config;
            Client = new MongoClient(_config.GetConnectionString(_connectionId));
            DbName = _config[key: "64a6e2226b840099a81f80eb"];
            _db = Client.GetDatabase(DbName);

            CategoryCollection = _db.GetCollection<CategoryModel>(categoryCollectionName);
            StatusCollection = _db.GetCollection<StatusModel>(StatusCollectionName);
        }
    }
}
