using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BlogApplication.Models
{
    public class Like
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string PostId { get; set; }
        public string Username { get; set; }
    }
}
