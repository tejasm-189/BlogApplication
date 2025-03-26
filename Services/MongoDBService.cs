using MongoDB.Driver;
using BlogApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using BlogApplication.Services;

namespace BlogApplication.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<BlogPost> _blogPosts;
        private readonly IMongoCollection<Follow> _follows; 
        private readonly string _uploadPath;
        private readonly ILogger<MongoDBService> _logger;
        private readonly INotificationService _notificationService;
        private readonly NotificationUpdateService _notificationUpdateService;

        public MongoDBService(
            IConfiguration configuration, 
            IWebHostEnvironment webHostEnvironment, 
            ILogger<MongoDBService> logger,
            INotificationService notificationService,
            NotificationUpdateService notificationUpdateService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _notificationUpdateService = notificationUpdateService;

            var connectionString = configuration.GetConnectionString("MongoDB");
            var databaseName = configuration.GetConnectionString("DatabaseName");

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException("MongoDB configuration is missing from appsettings.json");
            }

            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _blogPosts = database.GetCollection<BlogPost>("BlogPosts");
                _follows = database.GetCollection<Follow>("Follows"); 
                _logger.LogInformation("MongoDB connection established successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to MongoDB.");
                throw;
            }

            _uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
            Directory.CreateDirectory(_uploadPath);
        }

        public async Task<List<BlogPost>> GetAllAsync()
        {
            try
            {
                var posts = await _blogPosts.Find(_ => true).ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} blog posts.", posts.Count);
                return posts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog posts.");
                throw;
            }
        }

        public async Task<BlogPost> GetPostByIdAsync(string id)
        {
            try
            {
                var post = await _blogPosts.Find(post => post.Id == id).FirstOrDefaultAsync();
                _logger.LogInformation("Retrieved post with ID: {Id}", id);
                return post;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving post with ID: {Id}", id);
                throw;
            }
        }

        public async Task CreateAsync(BlogPost post)
        {
            try
            {
                await _blogPosts.InsertOneAsync(post);
                _logger.LogInformation("Created new post with ID: {Id}", post.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post.");
                throw;
            }
        }

        public async Task UpdateAsync(string id, BlogPost post)
        {
            try
            {
                var result = await _blogPosts.ReplaceOneAsync(p => p.Id == id, post);
                if (result.ModifiedCount > 0)
                    _logger.LogInformation("Updated post with ID: {Id}", id);
                else
                    _logger.LogWarning("No changes made to post with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating post with ID: {Id}", id);
                throw;
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                var result = await _blogPosts.DeleteOneAsync(p => p.Id == id);
                if (result.DeletedCount > 0)
                    _logger.LogInformation("Deleted post with ID: {Id}", id);
                else
                    _logger.LogWarning("No post found to delete with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post with ID: {Id}", id);
                throw;
            }
        }

        public async Task<string> UploadImageAsync(IBrowserFile file)
        {
            try
            {
                var fileName = $"{Guid.NewGuid()}_{file.Name}";
                var filePath = Path.Combine(_uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(stream);
                }
                _logger.LogInformation("Uploaded image: {FileName}", fileName);
                return $"/uploads/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image.");
                throw;
            }
        }

        public async Task AddLikeAsync(string postId, string userId, string username)
        {
            try
            {
                var like = new Like
                {
                    UserId = userId,
                    PostId = postId,
                    Username = username
                };
                var filter = Builders<BlogPost>.Filter.Eq(p => p.Id, postId);
                var update = Builders<BlogPost>.Update.AddToSet(p => p.Likes, like);
                var result = await _blogPosts.UpdateOneAsync(filter, update);
                if (result.ModifiedCount > 0)
                    _logger.LogInformation("Added like for PostId: {PostId}, UserId: {UserId}", postId, userId);
                else
                    _logger.LogWarning("Failed to add like or like already exists for PostId: {PostId}, UserId: {UserId}", postId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding like for PostId: {PostId}, UserId: {UserId}", postId, userId);
                throw;
            }
        }

        public async Task RemoveLikeAsync(string postId, string userId)
        {
            try
            {
                var filter = Builders<BlogPost>.Filter.Eq(p => p.Id, postId);
                var update = Builders<BlogPost>.Update.PullFilter(p => p.Likes, l => l.UserId == userId);
                var result = await _blogPosts.UpdateOneAsync(filter, update);
                if (result.ModifiedCount > 0)
                    _logger.LogInformation("Removed like for PostId: {PostId}, UserId: {UserId}", postId, userId);
                else
                    _logger.LogWarning("No like found to remove for PostId: {PostId}, UserId: {UserId}", postId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing like for PostId: {PostId}, UserId: {UserId}", postId, userId);
                throw;
            }
        }

        public async Task AddCommentAsync(string postId, Comment comment)
        {
            try
            {
                comment.PostId = postId;
                var filter = Builders<BlogPost>.Filter.Eq(p => p.Id, postId);
                var update = Builders<BlogPost>.Update.Push(p => p.Comments, comment);
                var result = await _blogPosts.UpdateOneAsync(filter, update);
                if (result.ModifiedCount > 0)
                    _logger.LogInformation("Added comment for PostId: {PostId}", postId);
                else
                    _logger.LogWarning("Failed to add comment for PostId: {PostId}", postId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment for PostId: {PostId}", postId);
                throw;
            }
        }

        public async Task DeleteCommentAsync(string postId, string commentId)
        {
            try
            {
                var filter = Builders<BlogPost>.Filter.Eq(p => p.Id, postId);
                var update = Builders<BlogPost>.Update.PullFilter(p => p.Comments, c => c.Id == commentId);
                var result = await _blogPosts.UpdateOneAsync(filter, update);

                if (result.ModifiedCount > 0)
                    _logger.LogInformation("Deleted comment with ID: {CommentId} from PostId: {PostId}", commentId, postId);
                else
                    _logger.LogWarning("No comment found to delete with ID: {CommentId} for PostId: {PostId}", commentId, postId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment with ID: {CommentId} from PostId: {PostId}", commentId, postId);
                throw;
            }
        }

        public async Task AddFollowAsync(string followerId, string followerUsername, string followerEmail, string followingId)
        {
            try
            {
                var follow = new Follow
                {
                    FollowerId = followerId,
                    FollowerUsername = followerUsername,
                    FollowerEmail = followerEmail,
                    FollowingId = followingId
                };
                await _follows.InsertOneAsync(follow);
                _logger.LogInformation("User {FollowerId} ({FollowerUsername}) followed {FollowingId}",
                                       followerId, followerUsername, followingId);
                              
                // Create a notification for the user being followed
                await _notificationService.CreateNotificationAsync(
                    userId: followingId,
                    actorId: followerId,
                    actorUsername: followerUsername,
                    actorEmail: followerEmail,
                    type: NotificationType.NewFollower,
                    message: $"{followerUsername} started following you"
                );
                
                // Notify components about the new notification
                _notificationUpdateService.NotifyNewNotification();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding follow from {FollowerId} to {FollowingId}", followerId, followingId);
                throw;
            }
        }

        public async Task RemoveFollowAsync(string followerId, string followingId)
        {
            try
            {
                var filter = Builders<Follow>.Filter.And(
                    Builders<Follow>.Filter.Eq(f => f.FollowerId, followerId),
                    Builders<Follow>.Filter.Eq(f => f.FollowingId, followingId)
                );
                var result = await _follows.DeleteOneAsync(filter);
                if (result.DeletedCount > 0)
                    _logger.LogInformation("User {FollowerId} unfollowed {FollowingId}", followerId, followingId);
                else
                    _logger.LogWarning("No follow relationship found to remove between {FollowerId} and {FollowingId}", followerId, followingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing follow from {FollowerId} to {FollowingId}", followerId, followingId);
                throw;
            }
        }

        public async Task<List<Follow>> GetFollowingAsync(string userId)
        {
            try
            {
                var following = await _follows.Find(f => f.FollowerId == userId).ToListAsync();
                _logger.LogInformation("Retrieved {Count} following for user {UserId}", following.Count, userId);
                return following;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving following list for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<Follow>> GetFollowersAsync(string userId)
        {
            try
            {
                var followers = await _follows.Find(f => f.FollowingId == userId).ToListAsync();
                _logger.LogInformation("Retrieved {Count} followers for user {UserId}", followers.Count, userId);
                return followers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving followers list for user {UserId}", userId);
                throw;
            }
        }

    }
}