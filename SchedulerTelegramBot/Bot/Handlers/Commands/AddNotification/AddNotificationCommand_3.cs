using MassTransit;
using MediatR;
using Quartz;
using SchedulerTelegramBot.Contracts;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Entities;
using SchedulerTelegramBot.Features.Notifications.Requests;
using SchedulerTelegramBot.Jobs;
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
    public class AddNotificationCommand_3(InfoStore infoStore, ISender sender) : MessageHandler
    {
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

                var storedData = infoStore.Get(chatId);

                ArgumentNullException.ThrowIfNull(storedData, nameof(storedData));

                ArgumentNullException.ThrowIfNull(storedData.Title, nameof(storedData.Title));

                await sender.Send(new CreateNotificationRequest(storedData.Title, chatId, taskNotifyDate, storedData.Description, null));

                container.DeleteEnumState<InputUserState>();

                return Result.Ok();
            }
            catch (Exception ex) 
            {
                await Reply("Could not add a task, some state variables were missing.", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());
                
                return Result.Fault();
            }
        }
    }
}
