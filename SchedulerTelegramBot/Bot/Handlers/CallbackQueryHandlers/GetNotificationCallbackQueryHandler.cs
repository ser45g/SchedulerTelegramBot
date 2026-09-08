using MediatR;
using SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes;
using SchedulerTelegramBot.Entities;
using SchedulerTelegramBot.Features.Notifications.Requests;
using SchedulerTelegramBot.Features.Notifications.Responses;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Handlers;

namespace SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers
{
    [CallbackQueryHandler]
    [CallbackStartsWithData("get-notif")]
    public class GetNotificationCallbackQueryHandler(ISender sender) : CallbackQueryHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<CallbackQuery> container, CancellationToken cancellationToken)
        {
            var callbackData = container.HandlingUpdate?.CallbackQuery?.Data;

            long? chatId = container.HandlingUpdate?.GetChatId();

            Guid? notificationId = null;

            if (callbackData != null)
            {
                if (Guid.TryParse(callbackData.Replace("get-notif-", ""), out var id))
                {
                    notificationId = id;
                }
            }

            if (notificationId == null || chatId == null)
            {
                await Responce("Something went wrong. Try again", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }

            var notificationDto = await sender.Send(new GetNotificationByIdRequest(notificationId.Value), cancellationToken);

            if (notificationDto == null)
            {
                await container.Responce("Could not find the notification", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }

            var notificationCard = NotificationCardHtmlText(notificationDto);

            await container.Responce(notificationCard, cancellationToken: cancellationToken,parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());

            return Result.Ok();
        }

        private string NotificationCardHtmlText(NotificationResponseDto notification)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"<b>Title:</b> {notification.Title}");

            if (notification.Description != null)
            {
                stringBuilder.AppendLine();

                stringBuilder.AppendLine($"<b>Description:</b> {notification.Description}");
            }

            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"<b>Notification Date:</b> {notification.NotifyDateTime.ToLongDateString()} {notification.NotifyDateTime.ToLongTimeString()}");

            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"<b>Added:</b> {notification.AddedDateTime}");

            return stringBuilder.ToString();
        }
    }
}
