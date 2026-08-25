using MediatR;
using Microsoft.EntityFrameworkCore;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Features.Notifications.Requests;
using SchedulerTelegramBot.Features.Notifications.Responses;
using SchedulerTelegramBot.Mappers;

namespace SchedulerTelegramBot.Features.Notifications.Handlers
{
    public class GetAllNotificationsForUserHandler(SchedulerDbContext context): IRequestHandler<GetAllNotificationsForUserRequest, IEnumerable<NotificationResponseDto>>
    {
        public async Task<IEnumerable<NotificationResponseDto>> Handle(GetAllNotificationsForUserRequest request, CancellationToken cancellationToken)
        {
            var notifications = await context.Notifications.AsNoTracking().Where(n=>n.ChatId==request.ChatId).ToListAsync(cancellationToken);
    
            return notifications.Select(notification => notification.ToNotificationResponseDto());
        }
    }
    
}
