using MediatR;
using SchedulerTelegramBot.Features.Notifications.Responses;

namespace SchedulerTelegramBot.Features.Notifications.Requests
{
    public record class UpdateNotificationRequest(Guid Id, string Title, long ChatId, DateTime NotifyDateTime, string? Description = null, TimeSpan? PeriodicNotificationPeriod = null): IRequest<NotificationResponseDto>;
}
