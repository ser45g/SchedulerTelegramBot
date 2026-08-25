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

                    await Responce($"Event was successfuly deleted!", cancellationToken: cancellation);
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
