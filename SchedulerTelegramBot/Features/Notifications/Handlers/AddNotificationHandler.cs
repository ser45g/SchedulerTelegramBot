using MediatR;
using Quartz;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Entities;
using SchedulerTelegramBot.Features.Notifications.Requests;
using SchedulerTelegramBot.Features.Notifications.Responses;
using SchedulerTelegramBot.Jobs;
using SchedulerTelegramBot.Mappers;

namespace SchedulerTelegramBot.Features.Notifications.Handlers
{
    public class AddNotificationHandler(SchedulerDbContext context, ISchedulerFactory schedulerFactory): IRequestHandler<CreateNotificationRequest, NotificationResponseDto>
    {
        public async Task<NotificationResponseDto> Handle(CreateNotificationRequest request, CancellationToken cancellationToken)
        {
            var notification = new Notification() {
                Title = request.Title,
                Description = request.Description,
                AddedAtUtc = DateTime.UtcNow,
                ChatId = request.ChatId,
                NotifyAtUtc= request.NotifyDateTime,
                PeriodicNotificationPeriod = request.PeriodicNotificationPeriod,
            };
    
            context.Notifications.Add(notification);
    
            await context.SaveChangesAsync(cancellationToken);
    
            IScheduler scheduler = await schedulerFactory.GetScheduler(cancellationToken);

            var jobData = new JobDataMap()
            {
                {"ChatId",request.ChatId },
                {"NotificationId", notification.Id },
            };

            IJobDetail job = JobBuilder.Create<NotifyUserJob>().UsingJobData(jobData).Build();
    
            ITrigger trigger = TriggerBuilder.Create().WithIdentity($"notify-user-{notification.Id}").ForJob(job).StartAt(notification.NotifyAtUtc).Build();
    
            await scheduler.ScheduleJob(job, trigger, cancellationToken);
    
            return notification.ToNotificationResponseDto();
        }
    }
}
