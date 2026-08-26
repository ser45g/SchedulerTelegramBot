using SchedulerTelegramBot.Bot.Stores;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Annotations;
using Telegrator.Annotations.StateKeeping;
using Telegrator.Handlers;
using Telegrator.StateKeeping;

namespace SchedulerTelegramBot.Bot.Handlers.Commands.AddNotification
{
    [MessageHandler]
    [ChatType(ChatType.Private)]
    [EnumState<AddNotificationCommandInputUserState>(AddNotificationCommandInputUserState.WaitingForDescription)]
    public class AddNotificationRequestNotificationDateTimeCommand : MessageHandler
    {
        private readonly AddNotificationCommandInfoStore _infoStore;

        public AddNotificationRequestNotificationDateTimeCommand(AddNotificationCommandInfoStore infoStore)
        {
            _infoStore = infoStore;
        }
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            var message = container.ActualUpdate;

            var description = message.Text== "<Skip>"? null: message.Text;
            try
            {
                long? chatId = container.HandlingUpdate.Message?.Chat.Id;

                ArgumentNullException.ThrowIfNull(chatId, nameof(chatId));

                var storedData = _infoStore.Get(chatId.Value);

                ArgumentNullException.ThrowIfNull(storedData, nameof(storedData));

                storedData.Description = description;

                _infoStore.Set(chatId.Value, storedData);

                await container.Reply($"Okay, now enter the date when you want to be notified. For example, 2026-01-10     02:00:00:", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());

                container.ForwardEnumState<AddNotificationCommandInputUserState>();
                
                return Result.Ok();

            }
            catch (Exception ex)
            {
                await container.Reply($"Could not process the request. Try again later", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();

            }


        }
    }

}
