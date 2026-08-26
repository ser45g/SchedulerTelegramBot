using MediatR;
using Quartz;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Features.Notifications.Requests;
using SchedulerTelegramBot.Features.Notifications.Responses;
using SchedulerTelegramBot.Jobs;
using SchedulerTelegramBot.Mappers;

namespace SchedulerTelegramBot.Features.Notifications.Handlers
{
    public class UpdateNotificationHandler(SchedulerDbContext dbContext, ISchedulerFactory    schedulerFactory) :IRequestHandler<UpdateNotificationRequest, NotificationResponseDto>
    {
        public async Task<NotificationResponseDto> Handle(UpdateNotificationRequest request, CancellationToken cancellationToken)
        {
            var notification = await dbContext.Notifications.FindAsync([request.Id], cancellationToken);

            if(notification == null)
                throw new Exception(nameof(notification));

            notification.Title = request.Title;
            notification.Description = request.Description;
            notification.LastUpdatedAtUtc = DateTime.UtcNow;
            notification.ChatId = request.ChatId;
            notification.NotifyAtUtc = request.NotifyDateTime;
            notification.PeriodicNotificationPeriod = request.PeriodicNotificationPeriod;

            await dbContext.SaveChangesAsync(cancellationToken);

            IScheduler scheduler = await schedulerFactory.GetScheduler(cancellationToken);
    
            await scheduler.UnscheduleJob(new TriggerKey($"notify-user-{request.Id}"), cancellationToken);
    
            var jobData = new JobDataMap()
            {
                {"ChatId",request.ChatId },
                {"NotificationId", request.Id },
            };
            IJobDetail job = JobBuilder.Create<NotifyUserJob>().UsingJobData(jobData).Build();
    
            ITrigger trigger = TriggerBuilder.Create().WithIdentity($"notify-user-{request.Id}").ForJob(job).StartAt(request.NotifyDateTime).Build();
    
            await scheduler.ScheduleJob(job, trigger, cancellationToken);
    
            return notification.ToNotificationResponseDto();
        }
    }
    
}
