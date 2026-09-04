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
    [CallbackContainsData("del-notif")]
    public class DeleteNotificationCallbackQueryHandler(ISender sender, ISendEndpointProvider sendEndpointProvider, SchedulerDbContext dbContext) : CallbackQueryHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<CallbackQuery> container, CancellationToken cancellationToken)
        {
            var callbackData = container.HandlingUpdate?.CallbackQuery?.Data;

            long? chatId = container.HandlingUpdate?.GetChatId();

            Guid? notificationId = null;

            if (callbackData != null && callbackData.StartsWith("del-notif-"))
            {
                var notificationIdString = callbackData.Replace("del-notif-", "");

                if (Guid.TryParse(notificationIdString, out var id))
                {
                    notificationId = id;
                }
            }

            if (notificationId == null || chatId == null) 
            {
                await Responce("Something went wrong. Try again", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());
                return Result.Fault();
            }

            await sender.Send(new DeleteNotificationRequest(notificationId.Value, chatId.Value), cancellationToken);

            return Result.Ok();
        }
    }
}
