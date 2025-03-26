using BlogApplication.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BlogApplication.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMongoCollection<Notification> _notifications;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IMongoClient mongoClient,
            ILogger<NotificationService> logger,
            IConfiguration configuration)
        {
            var databaseName = configuration.GetConnectionString("DatabaseName");
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException("Database name is missing from appsettings.json");
            }

            var database = mongoClient.GetDatabase(databaseName);
            _notifications = database.GetCollection<Notification>("Notifications");
            _logger = logger;
        }

        // Store notification in database and send real-time notification
        public async Task CreateNotificationAsync(
            string userId,
            string actorId,
            string actorUsername,
            string actorEmail,
            NotificationType type,
            string message)
        {
            try
            {
                // Create notification object
                var notification = new Notification
                {
                    UserId = userId,
                    ActorId = actorId,
                    ActorUsername = actorUsername,
                    ActorEmail = actorEmail,
                    Type = type,
                    Message = message,
                    CreatedAt = DateTime.UtcNow
                };

                // Store in database
                await _notifications.InsertOneAsync(notification);

                _logger.LogInformation(
                    "Sent notification to user {UserId} from {ActorUsername}: {Message}",
                    userId, actorUsername, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating notification for {UserId} from {ActorUsername}",
                    userId, actorUsername);
                throw;
            }
        }

        // Get notifications for a user
        public async Task<List<Notification>> GetUserNotificationsAsync(string userId, int limit = 20)
        {
            try
            {
                var notifications = await _notifications
                    .Find(n => n.UserId == userId)
                    .SortByDescending(n => n.CreatedAt)
                    .Limit(limit)
                    .ToListAsync();

                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications for user {UserId}", userId);
                throw;
            }
        }

        // Mark notification as read
        public async Task MarkAsReadAsync(string notificationId)
        {
            try
            {
                var filter = Builders<Notification>.Filter.Eq(n => n.Id, notificationId);
                var update = Builders<Notification>.Update.Set(n => n.IsRead, true);

                await _notifications.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
                throw;
            }
        }

        // Mark all user notifications as read
        public async Task MarkAllAsReadAsync(string userId)
        {
            try
            {
                var filter = Builders<Notification>.Filter.Eq(n => n.UserId, userId);
                var update = Builders<Notification>.Update.Set(n => n.IsRead, true);

                await _notifications.UpdateManyAsync(filter, update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
                throw;
            }
        }

        // Get unread notification count
        public async Task<int> GetUnreadCountAsync(string userId)
        {
            try
            {
                return (int)await _notifications
                    .CountDocumentsAsync(n => n.UserId == userId && !n.IsRead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notification count for user {UserId}", userId);
                throw;
            }
        }
    }
} 