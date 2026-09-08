using MassTransit;
using MediatR;
using SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes;
using SchedulerTelegramBot.Contracts;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Features.Notifications.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;

using Telegrator.Handlers;

namespace SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers
{
    [CallbackQueryHandler]
    [CallbackStartsWithData("del-notif")]
    public class DeleteNotificationCallbackQueryHandler(ISender sender) : CallbackQueryHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<CallbackQuery> container, CancellationToken cancellationToken)
        {
            var callbackData = container.HandlingUpdate?.CallbackQuery?.Data;

            long? chatId = container.HandlingUpdate?.GetChatId();

            Guid? notificationId = null;

            if (callbackData != null)
            {
                if (Guid.TryParse(callbackData.Replace("del-notif-", ""), out var id))
                {
                    notificationId = id;
                }
            }

            if (notificationId == null || chatId == null) 
            {
                await Responce("Something went wrong. Try again", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }
            try
            {
                await sender.Send(new DeleteNotificationRequest(notificationId.Value, chatId.Value), cancellationToken);

                return Result.Ok();
            }
            catch (Exception ex) 
            {
                await Responce("Could not delete the notification", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }
        }
    }
}
