using MassTransit;
using MassTransit.Transports;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SchedulerTelegramBot.Contracts;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Entities;
using SchedulerTelegramBot.Features.Notifications.Requests;
using Telegram.Bot.Types;

namespace SchedulerTelegramBot.Features.Notifications.Handlers
{
    public class DeleteNotificationHandler(ISendEndpointProvider sendEndpointProvider, SchedulerDbContext dbContext, ISchedulerFactory schedulerFactory) : IRequestHandler<DeleteNotificationRequest, bool>
    {
        public async Task<bool> Handle(DeleteNotificationRequest request, CancellationToken cancellationToken)
        {
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:send-response"));

            var notification = await dbContext.Notifications.FindAsync([request.Id], cancellationToken: cancellationToken);

            if (notification == null)
            {
                await sendEndpoint.Send(new SendResponse(request.ChatId, "Could not find the notification"), cancellationToken);

                return false;
            }

            dbContext.Notifications.Remove(notification);

            await sendEndpoint.Send(new SendResponse(request.ChatId, $"Notification <{notification.Title}> was successfully deleted"), cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            IScheduler scheduler = await schedulerFactory.GetScheduler(cancellationToken);

            await scheduler.UnscheduleJob(new TriggerKey($"notify-user-{request.Id}"), cancellationToken);

            return true;
        }
    }  
}
