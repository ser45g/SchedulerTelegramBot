namespace SchedulerTelegramBot.Features.Notifications.Responses
{
    public record class NotificationResponseDto(Guid Id, long ChatId, string Title, DateTime AddedDateTime, DateTime NotifyDateTime, string? Description=null, DateTime? LastUpdatedDateTime=null, TimeSpan? PeriodicNotificationPeriod=null);
    
}
