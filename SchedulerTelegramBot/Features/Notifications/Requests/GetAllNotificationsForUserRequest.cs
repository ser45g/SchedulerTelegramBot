using MediatR;
using SchedulerTelegramBot.Features.Notifications.Responses;

namespace SchedulerTelegramBot.Features.Notifications.Requests
{
    public record class GetAllNotificationsForUserRequest(long ChatId) : IRequest<IEnumerable<NotificationResponseDto>>;
}
