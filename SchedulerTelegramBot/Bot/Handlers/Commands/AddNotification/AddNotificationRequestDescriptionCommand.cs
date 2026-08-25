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
                container.ForwardEnumState<AddNotificationCommandInputUserState>();
                long? chatId = container.HandlingUpdate.Message?.Chat.Id;
            
                if (chatId == null)
                    throw new ArgumentNullException(nameof(chatId));

                var storedData = _infoStore.Get(chatId.Value);
                if (storedData == null)
                    storedData = new AddNotificationCommandInfoStore.StoreData();
                storedData.Title = title;

                _infoStore.Set(chatId.Value, storedData);

                await container.Reply($" Please enter the description (optional):",replyMarkup: new[] {"Skip"}, cancellationToken: cancellation);
            }
            else
            {
                await Reply("Title for a task cannot be empty. Try again", cancellationToken:cancellation, replyMarkup: new ReplyKeyboardRemove());

            }
                
            return Result.Ok();
        }
    }

}
