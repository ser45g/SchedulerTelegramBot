using MediatR;
using Microsoft.EntityFrameworkCore;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Features.Notifications.Requests;
using SchedulerTelegramBot.Features.Notifications.Responses;
using SchedulerTelegramBot.Mappers;

namespace SchedulerTelegramBot.Features.Notifications.Handlers
{
    public class GetNotificationHandler(SchedulerDbContext context): IRequestHandler<GetNotificationByIdRequest,NotificationResponseDto?>
    {
        public async Task<NotificationResponseDto?> Handle(GetNotificationByIdRequest request, CancellationToken cancellationToken)
        {
            var notification = await context.Notifications.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

            if (notification == null) {
                return null;
            }
            return notification.ToNotificationResponseDto();
        }
    }
}
