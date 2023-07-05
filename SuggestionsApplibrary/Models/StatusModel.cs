
namespace SuggestionsApplibrary.Models;
    public class StatusModel
    {
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }

    public string StatusName { get; set; }
     
    public string StatusDescription { get; set; }
}

