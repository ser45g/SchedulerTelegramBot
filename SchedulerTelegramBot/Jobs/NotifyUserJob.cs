using Quartz;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Entities;
using Telegram.Bot;

namespace SchedulerTelegramBot.Jobs
{
    public class NotifyUserJob(ITelegramBotClient telegramBotClient, SchedulerDbContext schedulerDbContext) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap jobData = context.MergedJobDataMap;

            long? chatId = jobData.GetLong("ChatId");

            if (chatId == null)
                throw new ArgumentNullException(nameof(chatId));

            Guid? notificationId = jobData.GetGuid("NotificationId");

            if (notificationId==null)
                throw new ArgumentNullException(nameof(notificationId));

            var notification = await schedulerDbContext.Notifications.FindAsync(notificationId);

            if (notification == null)
                return;

            await telegramBotClient.SendMessage(chatId, $"It's time for your task: {notification.Title}");
        }
    }
}
