using MongoDB.Driver;
using SuggestionsApplibrary.Models;

namespace SuggestionsApplibrary.DataAccess
{
    public interface IDbConnection
    {
        IMongoCollection<CategoryModel> CategoryCollection { get; }
        string categoryCollectionName { get; }
        MongoClient Client { get; }
        string DbName { get; }
        IMongoCollection<StatusModel> StatusCollection { get; }
        string StatusCollectionName { get; }
        IMongoCollection<SuggestionModel> SuggestioinCollection { get; }
        string SuggestionCollectionName { get; }
        IMongoCollection<UserModel> UserCollection { get; }
        string UserCollectionName { get; }
    }
}