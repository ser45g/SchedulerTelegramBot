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
    [CommandAllias("get_notifications")]
    public class GetNotificationsForUserCommand : CommandHandler
    {
        private readonly ISender _sender;

        public GetNotificationsForUserCommand(ISender sender)
        {
            _sender = sender;
        }

        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellationToken)
        {
            var chatId = container.HandlingUpdate.Message?.Chat.Id;

            if (chatId == null)
            {
                await Reply("Something went wrong. Try again", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());
                return Result.Fault();
            }

            var notifications = await _sender.Send(new GetAllNotificationsForUserRequest(chatId.Value), cancellationToken: cancellationToken);

            var buttons = new List<InlineKeyboardButton[]> { };

            foreach (var notification in notifications)
            {
                string mark = notification.NotifyDateTime >= DateTime.Now ? "❌" : "✔";

                buttons.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"{notification.Title} - {mark}", $"get-notif-{notification.Id}") });
            }
            await Responce("""
                Here is a list of all your notificaions:
                """, replyMarkup: new InlineKeyboardMarkup(buttons.ToArray()), cancellationToken:cancellationToken);

            return Result.Ok();
        }
    }
}
