using Telegrator.Annotations;

namespace SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes
{
    public class CallbackStartsWithDataAttribute(string data): CallbackQueryAttribute(new CallbackStartsWithDataFilter(data)){ }
}
