using MediatR;

namespace SchedulerTelegramBot.Features.Notifications.Requests
{
    public record class DeleteNotificationRequest(Guid Id) : IRequest<bool>;
}
