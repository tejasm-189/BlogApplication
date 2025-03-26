using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BlogApplication.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string PostId { get; set; }
    }
}
