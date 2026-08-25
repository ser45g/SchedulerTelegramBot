using MediatR;
using SchedulerTelegramBot.Features.Notifications.Responses;

namespace SchedulerTelegramBot.Features.Notifications.Requests
{
    public record class GetNotificationByIdRequest(Guid Id) : IRequest<NotificationResponseDto>;
}
