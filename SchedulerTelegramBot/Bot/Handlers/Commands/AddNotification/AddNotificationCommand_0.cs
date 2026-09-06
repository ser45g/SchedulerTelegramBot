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
    public class AddNotificationCommand_0 : CommandHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellationToken)
        {

            await Responce("To add a notification, we'll need some information. What is the task name?", cancellationToken:cancellationToken, replyMarkup: new ReplyKeyboardRemove());

            container.ForwardEnumState<AddNotificationInputUserState>();

            return Result.Ok();
        }
    }

}
