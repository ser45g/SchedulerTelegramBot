using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SchedulerTelegramBot.Contracts.Messaging.Commands;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Entities;
using SchedulerTelegramBot.Features.Notifications.Requests;
using SchedulerTelegramBot.Features.Notifications.Responses;
using SchedulerTelegramBot.Mappers;

namespace SchedulerTelegramBot.Features.Notifications.Handlers
{
    public class AddNotificationHandler(SchedulerDbContext dbContext, ISendEndpointProvider sendEndpointProvider, IMessageScheduler scheduler, ILogger<AddNotificationHandler> logger): IRequestHandler<CreateNotificationRequest, NotificationResponseDto?>
    {
        public async Task<NotificationResponseDto?> Handle(CreateNotificationRequest request, CancellationToken cancellationToken)
        {
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:send-response"));

            var job = await scheduler.ScheduleSend(new Uri("queue:send-response"), request.NotifyDateTime, new SendResponse(request.ChatId, $"Notification for task: {request.Title}"), cancellationToken);

            var notification = new Notification() {
                Id = Guid.NewGuid(),
                ScheduledJobId = job.TokenId,
                Title = request.Title,
                Description = request.Description,
                AddedAtUtc = DateTime.UtcNow,
                ChatId = request.ChatId,
                NotifyAtUtc= request.NotifyDateTime.ToUniversalTime(),
                PeriodicNotificationPeriod = request.PeriodicNotificationPeriod,
            };
    
            dbContext.Notifications.Add(notification);

            await sendEndpoint.Send(new SendResponse(request.ChatId, "Notification was successfully added"), cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return notification.ToNotificationResponseDto();
        }
    }
}
