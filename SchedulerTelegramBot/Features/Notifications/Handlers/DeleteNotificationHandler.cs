using MediatR;
using Microsoft.EntityFrameworkCore;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Features.Notifications.Requests;

namespace SchedulerTelegramBot.Features.Notifications.Handlers
{
    public class DeleteNotificationHandler(SchedulerDbContext context) : IRequestHandler<DeleteNotificationRequest, bool>
    {
        public async Task<bool> Handle(DeleteNotificationRequest request, CancellationToken cancellationToken)
        {
            var rowsAffected = await context.Notifications.Where(n=>n.Id == request.Id).ExecuteDeleteAsync  (cancellationToken:cancellationToken);
    
            return rowsAffected == 1;
        }
    }  
}
