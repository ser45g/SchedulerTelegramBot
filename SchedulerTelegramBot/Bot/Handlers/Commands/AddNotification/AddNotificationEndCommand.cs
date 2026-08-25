using MediatR;
using SchedulerTelegramBot.Bot.Stores;
using SchedulerTelegramBot.Features.Notifications.Requests;
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
    [EnumState<AddNotificationCommandInputUserState>(AddNotificationCommandInputUserState.WaitingForNotificationDate)]
    public class AddNotificationEndCommand : MessageHandler
    {
        private readonly AddNotificationCommandInfoStore _infoStore;

        private readonly ISender _sender;

        public AddNotificationEndCommand(ISender sender, AddNotificationCommandInfoStore infoStore)
        {
            _sender = sender;
            _infoStore = infoStore;
        }
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            var message = container.ActualUpdate;

            var notificationDateString = message.Text;

            if(DateTime.TryParse(notificationDateString, out DateTime taskNotifyDate))
            {        
                long? chatId = container.HandlingUpdate.Message?.Chat.Id;

                if(chatId == null)
                    throw new ArgumentNullException(nameof(chatId));
                

                var storedData = _infoStore.Get(chatId.Value);

                if (storedData == null)
                    throw new ArgumentNullException(nameof(storedData));

                storedData.NotifyDate = taskNotifyDate;

                _infoStore.Set(chatId.Value, storedData);
                
                if(storedData.Title == null)
                    throw new ArgumentNullException(nameof(storedData.Title));

                await _sender.Send(new CreateNotificationRequest(storedData.Title, chatId.Value, taskNotifyDate, storedData.Description));

                container.DeleteEnumState<AddNotificationCommandInputUserState>();

                await Reply("The task was successfully added!", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());                
            }
            else
            {
                await Reply("Nofity date was invalid. Try again", cancellationToken:cancellation, replyMarkup: new ReplyKeyboardRemove());

            }
         
            return Result.Ok();
        }
    }

}
