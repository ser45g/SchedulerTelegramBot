using SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes;
using SchedulerTelegramBot.Bot.Handlers.Commands.UpdateNotification;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Handlers;
using Telegrator.StateKeeping;

namespace SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers
{
    [CallbackQueryHandler]
    [CallbackStartsWithData("update-notif")]
    public class UpdateNotificationCallbackQueryHandler(UpdateNotificationInfoStore infoStore) : CallbackQueryHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<CallbackQuery> container, CancellationToken cancellationToken)
        {
            var callbackData = container.HandlingUpdate?.CallbackQuery?.Data;

            long? chatId = container.HandlingUpdate?.GetChatId();

            Guid? notificationId = null;

            if (callbackData != null)
            {
                if (Guid.TryParse(callbackData.Replace("update-notif-", ""), out var id))
                {
                    notificationId = id;
                }
            }

            if (notificationId == null || chatId == null)
            {
                await Responce("Something went wrong. Try again later", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }
            try
            {
                var storedData = infoStore.Get(chatId.Value);

                storedData ??= new UpdateNotificationInfoStore.StoreData();

                storedData.Id = notificationId;

                infoStore.Set(chatId.Value, storedData);

                container.ForwardEnumState<UpdateNotificationInputUserState>();

                await container.Responce($"We're going to update {notificationId}. Enter title:", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());

                return Result.Ok();
            }
            catch (Exception ex)
            {
                await container.Responce($"Could not process the message", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }
        }
    }
}
