using Quartz;
using SchedulerTelegramBot.Data;
using Telegram.Bot;

namespace SchedulerTelegramBot.Jobs
{
    public class NotifyUserJob(ITelegramBotClient telegramBotClient, SchedulerDbContext schedulerDbContext) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap jobData = context.MergedJobDataMap;

            long? chatId = jobData.GetLong("ChatId");

            Guid? notificationId = jobData.GetGuid("NotificationId");

            if (notificationId == null || chatId == null)
                return;

            var notification = await schedulerDbContext.Notifications.FindAsync(notificationId, context.CancellationToken);

            if (notification == null)
                return;

            await telegramBotClient.SendMessage(chatId, $"It's time for your task: {notification.Title}");
        }
    }
}
