using SchedulerTelegramBot.Entities;
using SchedulerTelegramBot.Features.Notifications.Responses;

namespace SchedulerTelegramBot.Mappers
{
    public static class NotificationMappers
    {
        public static NotificationResponseDto ToNotificationResponseDto(this Notification notification)
        {
            return new NotificationResponseDto(notification.Id, notification.ChatId, notification.Title, notification.AddedDateTime, notification.NotifyDateTime, notification.Description, notification.LastUpdatedDateTime, notification.PeriodicNotificationPeriod);
        }
    }
}
