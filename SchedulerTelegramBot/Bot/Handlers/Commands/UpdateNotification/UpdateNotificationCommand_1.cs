using MediatR;
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
    [EnumState<UpdateNotificationInputUserState>(UpdateNotificationInputUserState.WaitingForTitle)]
    public class UpdateNotificationCommand_1(UpdateNotificationInfoStore infoStore) : MessageHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            long chatId = container.ActualUpdate.Chat.Id;

            var title = container.HandlingUpdate.Message?.Text;

            if (string.IsNullOrWhiteSpace(title))
            {
                await Reply("Title was invalid. Try again", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }

            try
            {
                var storedData = infoStore.Get(chatId);

                storedData ??= new UpdateNotificationInfoStore.StoreData();

                storedData.Title = title;

                await container.Reply($"Please enter the description (optional):", replyMarkup: new[] { "<Skip>" }, cancellationToken: cancellation);

                infoStore.Set(chatId, storedData);

                container.ForwardEnumState<UpdateNotificationInputUserState>();

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
