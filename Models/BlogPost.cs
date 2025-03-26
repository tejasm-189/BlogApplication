using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogApplication.Models
{
    public class Follow
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = String.Empty;

        public string FollowerId { get; set; } = string.Empty; 
        public string FollowerUsername { get; set; } = string.Empty; 
        public string FollowerEmail { get; set; } = string.Empty;
        public string FollowingId { get; set; } = string.Empty; 
        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }

    public class BlogPost
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = String.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool PublishNow { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<Like> Likes { get; set; } = new List<Like>();
        public string UserId { get; set; } = string.Empty;
        public bool IsExpanded { get; set; }
        public bool ShowComments { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }

}