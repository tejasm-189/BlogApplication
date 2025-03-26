using BlogApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogApplication.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(
            string userId,
            string actorId,
            string actorUsername,
            string actorEmail,
            NotificationType type,
            string message);
            
        Task<List<Notification>> GetUserNotificationsAsync(string userId, int limit = 20);
        
        Task MarkAsReadAsync(string notificationId);
        
        Task MarkAllAsReadAsync(string userId);
        
        Task<int> GetUnreadCountAsync(string userId);
    }
} 