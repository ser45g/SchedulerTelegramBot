using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Features.Notifications.Requests;

namespace SchedulerTelegramBot.Features.Notifications.Handlers
{
    public class DeleteNotificationHandler(SchedulerDbContext context, ISchedulerFactory schedulerFactory) : IRequestHandler<DeleteNotificationRequest, bool>
    {
        public async Task<bool> Handle(DeleteNotificationRequest request, CancellationToken cancellationToken)
        {
            var rowsAffected = await context.Notifications.Where(n=>n.Id == request.Id).ExecuteDeleteAsync  (cancellationToken:cancellationToken);

            IScheduler scheduler = await schedulerFactory.GetScheduler(cancellationToken);

            await scheduler.UnscheduleJob(new TriggerKey($"notify-user-{request.Id}"), cancellationToken);

            return rowsAffected == 1;
        }
    }  
}
