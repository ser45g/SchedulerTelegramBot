using Telegrator.Annotations;

namespace SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes
{

    public class CallbackContainsDataAttribute(string data): CallbackQueryAttribute(new CallbackContainsDataFilter(data)){ }
}
