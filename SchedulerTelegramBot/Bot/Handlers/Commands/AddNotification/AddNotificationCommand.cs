using SchedulerTelegramBot.Bot.Aspects;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Annotations;
using Telegrator.Aspects;
using Telegrator.Handlers;
using Telegrator.StateKeeping;

namespace SchedulerTelegramBot.Bot.Handlers.Commands.AddNotification
{

    [CommandHandler]
    [BeforeExecution<CleanUpAddNotificationInputUserStatePreProcessor>]
    [CommandAllias("add_notification")]
    public class AddNotificationRequestTitleCommand : CommandHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            container.ForwardEnumState<AddNotificationCommandInputUserState>();

            await Responce("""
                To add a notification, we'll need some information. What is the task name?
                """, cancellationToken:cancellation, replyMarkup: new ReplyKeyboardRemove());

            return Result.Ok();
        }
    }

}
