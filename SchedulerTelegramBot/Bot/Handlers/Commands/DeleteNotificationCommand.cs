using MediatR;
using SchedulerTelegramBot.Features.Notifications.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Annotations;
using Telegrator.Handlers;

namespace SchedulerTelegramBot.Bot.Handlers.Commands
{
  
    [CommandHandler]
    [CommandAllias("delete_notification")]
    public class DeleteNotificationCommand(ISender sender) : CommandHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellationToken)
        {
            long chatId = container.ActualUpdate.Chat.Id;

            var notifications = await sender.Send(new GetAllNotificationsForUserRequest(chatId), cancellationToken: cancellationToken);

            var buttons = new List<InlineKeyboardButton[]> { };
           
            foreach (var notification in notifications)
            {
                string mark = notification.NotifyDateTime >= DateTime.Now ? "❌" : "✔";

                buttons.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"{notification.Title} - {mark}", $"del-notif-{notification.Id}") });
            }

            if (buttons.Count > 0)
            {
                await Responce("Here is a list of all your notificaions. Click on one to delete it", replyMarkup: new InlineKeyboardMarkup(buttons.ToArray()), cancellationToken: cancellationToken);
            }
            else
            {
                await Responce("No notifications exist at this point. Go to /add_notification to add one", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
            }

            return Result.Ok();
        }
    }
}
