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
    [CallbackContainsData("update-notif")]
    public class UpdateNotificationCallbackQueryHandler(InfoStore infoStore) : CallbackQueryHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<CallbackQuery> container, CancellationToken cancellationToken)
        {
            var callbackData = container.HandlingUpdate?.CallbackQuery?.Data;

            long? chatId = container.HandlingUpdate?.GetChatId();

            Guid? notificationId = null;

            if (callbackData != null && callbackData.StartsWith("update-notif-"))
            {
                var notificationIdString = callbackData.Replace("update-notif-", "");

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
            try
            {

                var storedData = infoStore.Get(chatId.Value);

                storedData ??= new InfoStore.StoreData();

                storedData.Id = notificationId;

                infoStore.Set(chatId.Value, storedData);

                container.ForwardEnumState<InputUserState>();

                await container.Responce($"We're going to update {notificationId}. Enter title:");

                return Result.Ok();
            }
            catch (Exception ex)
            {
                await container.Responce($"Could not process the message");

                return Result.Fault();
            }

        }
    }
}
