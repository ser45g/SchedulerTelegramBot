using MassTransit;
using MediatR;
using Quartz;
using SchedulerTelegramBot.Contracts.Messaging.Commands;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Features.Notifications.Requests;

namespace SchedulerTelegramBot.Features.Notifications.Handlers
{
    public class DeleteNotificationHandler(ISendEndpointProvider sendEndpointProvider, SchedulerDbContext dbContext, IMessageScheduler scheduler) : IRequestHandler<DeleteNotificationRequest, bool>
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
            await scheduler.CancelScheduledPublish<SendResponse>(notification.ScheduledJobId, cancellationToken: cancellationToken);

            dbContext.Notifications.Remove(notification);

            await sendEndpoint.Send(new SendResponse(request.ChatId, $"Notification <{notification.Title}> was successfully deleted"), cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            
            return true;
        }
    }  
}
