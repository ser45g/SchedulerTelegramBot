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
    [EnumState<AddNotificationInputUserState>(AddNotificationInputUserState.WaitingForTitle)]
    public class AddNotificationCommand_1 : MessageHandler
    {
        private readonly AddNotificationInfoStore _infoStore;

        public AddNotificationCommand_1(AddNotificationInfoStore infoStore)
        {
            _infoStore = infoStore;
        }
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellationToken)
        {
            var message = container.ActualUpdate;

            var title = message.Text;

            if (string.IsNullOrWhiteSpace(title))
            {
                await Reply("Title was invalid. Try again", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }

            try
            {
                long chatId = container.ActualUpdate.Chat.Id;

                var storedData = _infoStore.Get(chatId);

                storedData ??= new AddNotificationInfoStore.StoreData();

                storedData.Title = title;

                await container.Reply($"Please enter the description (optional):", replyMarkup: new[] { "<Skip>" }, cancellationToken: cancellationToken);

                _infoStore.Set(chatId, storedData);

                container.ForwardEnumState<AddNotificationInputUserState>();
                
                return Result.Ok();
            }
            catch (Exception ex) 
            {
                await Reply("Could not process the message. Please try again later", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }
        }
    }

}
