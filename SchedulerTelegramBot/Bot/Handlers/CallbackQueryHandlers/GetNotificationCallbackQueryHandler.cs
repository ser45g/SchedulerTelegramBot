using MediatR;
using SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes;
using SchedulerTelegramBot.Features.Notifications.Requests;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Handlers;

namespace SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers
{
    [CallbackQueryHandler]
    [CallbackContainsData("get-notif")]
    public class GetNotificationCallbackQueryHandler(ISender sender) : CallbackQueryHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<CallbackQuery> container, CancellationToken cancellation)
        {
            var callbackData = container.HandlingUpdate?.CallbackQuery?.Data;

            long? chatId = container.HandlingUpdate?.GetChatId();

            Guid? notificationId = null;

            if (callbackData != null && callbackData.StartsWith("get-notif-"))
            {
                var notificationIdString = callbackData.Replace("get-notif-", "");

                if (Guid.TryParse(notificationIdString, out var id))
                {
                    notificationId = id;
                }
            }

            if (notificationId == null || chatId == null)
            {
                await Responce("Something went wrong. Try again", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }

            var notification = await sender.Send(new GetNotificationByIdRequest(notificationId.Value), cancellation);

            if (notification == null)
            {
                await container.Responce("Could not find the notification", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Title: {notification.Title}");

            if (notification.Description != null)
            {
                stringBuilder.AppendLine($"Description: {notification.Description}");
            }

            stringBuilder.AppendLine($"Notification Date: {notification.NotifyDateTime.ToLongDateString()} {notification.NotifyDateTime.ToLongTimeString()}");

            stringBuilder.AppendLine($"Added: {notification.AddedDateTime}");

            await Responce(stringBuilder.ToString(), cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());

            return Result.Ok();
        }
    }
}
