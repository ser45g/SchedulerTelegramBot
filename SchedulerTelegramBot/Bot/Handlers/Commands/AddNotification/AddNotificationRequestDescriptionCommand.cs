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
    [EnumState<AddNotificationCommandInputUserState>(AddNotificationCommandInputUserState.WaitingForTitle)]
    public class AddNotificationRequestDescriptionCommand : MessageHandler
    {
        private readonly AddNotificationCommandInfoStore _infoStore;

        public AddNotificationRequestDescriptionCommand(AddNotificationCommandInfoStore infoStore)
        {
            _infoStore = infoStore;
        }
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            var message = container.ActualUpdate;

            var title = message.Text;

            if (!string.IsNullOrWhiteSpace(title))
            {
                await Reply("Title was invalid. Try again", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }

            try
            {
                long? chatId = container.HandlingUpdate.Message?.Chat.Id;

                ArgumentNullException.ThrowIfNull(chatId, nameof(chatId));

                var storedData = _infoStore.Get(chatId.Value);

                ArgumentNullException.ThrowIfNull(storedData, nameof(storedData));

                storedData.Title = title;

                await container.Reply($" Please enter the description (optional):", replyMarkup: new[] { "<Skip>" }, cancellationToken: cancellation);

                _infoStore.Set(chatId.Value, storedData);

                container.ForwardEnumState<AddNotificationCommandInputUserState>();
                
                return Result.Ok();

            }
            catch (Exception ex) 
            {
                await Reply("Could not process the message. Please try again later", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }
        }
    }

}
