using SchedulerTelegramBot.Bot.Handlers.Commands.AddNotification;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Annotations;
using Telegrator.Annotations.StateKeeping;
using Telegrator.Handlers;
using Telegrator.StateKeeping;

namespace SchedulerTelegramBot.Bot.Handlers.Commands.UpdateNotification
{
    [MessageHandler]
    [ChatType(ChatType.Private)]
    [EnumState<InputUserState>(InputUserState.WaitingForDescription)]
    public class UpdateNotificationCommand_2(InfoStore infoStore) : MessageHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            var message = container.HandlingUpdate.Message;

            var description = message?.Text == "<Skip>" ? null : message?.Text;

            try
            {
                long chatId = container.ActualUpdate.Chat.Id;

                var storedData = infoStore.Get(chatId);

                ArgumentNullException.ThrowIfNull(storedData, nameof(storedData));

                storedData.Description = description;

                infoStore.Set(chatId, storedData);

                container.ForwardEnumState<InputUserState>();

                await container.Reply($"Okay, now enter the date when you want to be notified. For example, {DateTime.Now}", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());

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
