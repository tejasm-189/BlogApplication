using System;

namespace BlogApplication.Services
{
    public class NotificationUpdateService
    {
        // Event to notify components when notifications should be refreshed
        public event Action OnNotificationReceived;

        // Method to trigger notification refresh
        public void NotifyNewNotification()
        {
            OnNotificationReceived?.Invoke();
        }
    }
} 