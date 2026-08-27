using MediatR;
using SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes;
using SchedulerTelegramBot.Features.Notifications.Handlers;
using SchedulerTelegramBot.Features.Notifications.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;

using Telegrator.Handlers;

namespace SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers
{
    [CallbackQueryHandler]
    [CallbackContainsData("del-notif")]
    public class DeleteNotificationCallbackQueryHandler(ISender sender) : CallbackQueryHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<CallbackQuery> container, CancellationToken cancellation)
        {
            var callbackData = container.HandlingUpdate?.CallbackQuery?.Data;

            if (callbackData != null && callbackData.StartsWith("del-notif-"))
            {
                var notificationIdString = callbackData.Replace("del-notif-", "");

                if (Guid.TryParse(notificationIdString, out var notificationId))
                {
                    await sender.Send(new DeleteNotificationRequest(notificationId), cancellation);

                    long? chatId = container.ActualUpdate?.Message?.Chat.Id;

                    var buttons = new List<InlineKeyboardButton[]> { };

                    if (chatId != null)
                    {
                        var notifications = await sender.Send(new GetAllNotificationsForUserRequest(chatId.Value), cancellationToken:   cancellation);


                        foreach (var notification in notifications)
                        {
                            string mark = notification.NotifyDateTime >= DateTime.Now ? "❌" : "✔";

                            buttons.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"{notification.Title} -  {mark}", $"del-notif-{notification.Id}") });
                        }
                    }

                    await Responce($"Event was successfuly deleted! Remaining events: {buttons.Count}", replyMarkup: buttons.ToArray(), cancellationToken: cancellation);


                }
                else
                {
                    await Responce("Something went wrong. Try again", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());
                }
            }

            return Result.Ok();
        }
    }
}
