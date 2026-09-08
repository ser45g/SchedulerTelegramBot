using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Annotations;
using Telegrator.Handlers;

namespace SchedulerTelegramBot.Bot.Handlers.Commands
{
    [CommandHandler]
    [CommandAllias("start")]
    public class StartCommand : CommandHandler
    {        
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Responce("""
                Hello, Welcome to this bot! <i>It may help with organizing your time</i>.
                This bot allows you to schedule some task for the time you want and when that time comes you'll recieve a notification. Send /add_notification for that.
                """, parseMode:ParseMode.Html, replyMarkup: new ReplyKeyboardRemove(), cancellationToken:cancellation);

            return Result.Ok();
        }
    }
}
