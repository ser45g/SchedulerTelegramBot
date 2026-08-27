using MediatR;
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
    [EnumState<InputUserState>(InputUserState.WaitingForNotificationDate)]
    public class AddNotificationCommand_3 : MessageHandler
    {
        private readonly InfoStore _infoStore;

        private readonly ISender _sender;

        public AddNotificationCommand_3(ISender sender, InfoStore infoStore)
        {
            _sender = sender;
            _infoStore = infoStore;
        }
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            var message = container.ActualUpdate;

            var notificationDateString = message.Text;

            if (!DateTime.TryParse(notificationDateString, out DateTime taskNotifyDate))
            {
                await Reply("Nofity date was invalid. Try again", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());
                return Result.Fault();
            }

            try
            {
                long chatId = container.ActualUpdate.Chat.Id;

                var storedData = _infoStore.Get(chatId);

                ArgumentNullException.ThrowIfNull(storedData, nameof(storedData));

                ArgumentNullException.ThrowIfNull(storedData.Title, nameof(storedData.Title));

                await _sender.Send(new CreateNotificationRequest(storedData.Title, chatId, taskNotifyDate, storedData.Description));
                
                container.DeleteEnumState<InputUserState>();
                
                //need to add outbox, it may fail to send that message
                await Reply("The task was successfully added!", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());

                return Result.Ok();

            }
            catch (Exception ex) {
                //same
                await Reply("Could not add a task", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());
                return Result.Fault();
            }
        }
    }
}
