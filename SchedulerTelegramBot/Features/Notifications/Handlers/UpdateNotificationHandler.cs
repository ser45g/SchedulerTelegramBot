using MassTransit;
using MediatR;
using SchedulerTelegramBot.Contracts.Messaging.Commands;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Features.Notifications.Requests;
using SchedulerTelegramBot.Features.Notifications.Responses;
using SchedulerTelegramBot.Mappers;

namespace SchedulerTelegramBot.Features.Notifications.Handlers
{
    public class UpdateNotificationHandler(SchedulerDbContext dbContext, IMessageScheduler scheduler, ISendEndpointProvider sendEndpointProvider) :IRequestHandler<UpdateNotificationRequest, NotificationResponseDto>
    {
        public async Task<NotificationResponseDto> Handle(UpdateNotificationRequest request, CancellationToken cancellationToken)
        {
            var notification = await dbContext.Notifications.FindAsync([request.Id], cancellationToken);

            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:send-response"));

            if (notification == null)
                throw new Exception(nameof(notification));

            await scheduler.CancelScheduledPublish<SendResponse>(notification.ScheduledJobId, cancellationToken: cancellationToken);
            
            var job = await scheduler.SchedulePublish(request.NotifyDateTime, new SendResponse(notification.ChatId, $"Time for your task: {notification.Title}"), cancellationToken: cancellationToken);

            notification.Title = request.Title;
            notification.Description = request.Description;
            notification.LastUpdatedAtUtc = DateTime.UtcNow;
            notification.ChatId = request.ChatId;
            notification.NotifyAtUtc = request.NotifyDateTime;
            notification.ScheduledJobId = job.TokenId;
            notification.PeriodicNotificationPeriod = request.PeriodicNotificationPeriod;

            await sendEndpoint.Send(new SendResponse(request.ChatId, $"The notification <{notification.Title}> was updated!"), cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return notification.ToNotificationResponseDto();
        }
    }
}
