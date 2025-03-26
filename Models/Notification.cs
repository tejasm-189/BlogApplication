using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BlogApplication.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        
        public string UserId { get; set; } // The user who will receive the notification
        
        public string ActorId { get; set; } // The user who triggered the notification
        
        public string ActorUsername { get; set; } // Username of the actor
        
        public string ActorEmail { get; set; } // Email of the actor
        
        public NotificationType Type { get; set; }
        
        public string Message { get; set; }
        
        public bool IsRead { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public enum NotificationType
    {
        NewFollower,
        NewLike,
        NewComment
    }
} 