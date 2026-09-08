using MediatR;
using SchedulerTelegramBot.Bot.Aspects;
using SchedulerTelegramBot.Features.Notifications.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Annotations;
using Telegrator.Aspects;
using Telegrator.Handlers;

namespace SchedulerTelegramBot.Bot.Handlers.Commands.UpdateNotification
{
  
    [CommandHandler]
    [BeforeExecution<CleanUpUpdateNotificationInputUserStatePreProcessor>()]
    [CommandAllias("update_notification")]
    public class UpdateNotificationCommand_0(ISender sender) : CommandHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            long chatId = container.ActualUpdate.Chat.Id;

            var notifications = await sender.Send(new GetAllNotificationsForUserRequest(chatId), cancellationToken: cancellation);

            var buttons = new List<InlineKeyboardButton[]> { };
           
            foreach (var notification in notifications)
            {
                string mark = notification.NotifyDateTime >= DateTime.Now ? "❌" : "✔";

                buttons.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"{notification.Title} - {mark}", $"update-notif-{notification.Id}") });
            }

            await Responce("Here is a list of notifications to update. Click on one to update it.", replyMarkup: buttons.ToArray(), cancellationToken: cancellation);

            return Result.Ok();
        }
    }
}
