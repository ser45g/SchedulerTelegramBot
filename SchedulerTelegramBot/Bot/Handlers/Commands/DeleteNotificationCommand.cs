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
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            long? chatId = container.HandlingUpdate.Message?.Chat.Id;

            if (chatId == null)
            {
                await Reply("Something went wrong. Try again", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }

            var notifications = await sender.Send(new GetAllNotificationsForUserRequest(chatId.Value), cancellationToken: cancellation);

            var buttons = new List<InlineKeyboardButton[]> { };
           
            foreach (var notification in notifications)
            {
                string mark = notification.NotifyDateTime >= DateTime.Now ? "❌" : "✔";

                buttons.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"{notification.Title} - {mark}", $"del-notif-{notification.Id}") });
            }

            await Responce("""
                Here is a list of notifications to delete. Click on one to delete it.
                """, replyMarkup: buttons.ToArray());

            return Result.Ok();
        }
    }
}
