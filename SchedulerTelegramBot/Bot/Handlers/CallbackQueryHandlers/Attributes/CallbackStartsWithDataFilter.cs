using Telegram.Bot.Types;
using Telegrator.Filters;
using Telegrator.Filters.Components;

namespace SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes
{
    public class CallbackStartsWithDataFilter : Filter<CallbackQuery>
    {
        private readonly string _data;

        public CallbackStartsWithDataFilter(string data)
        {
            _data = data;
        }

        public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
        {
            return context.Input.Data?.StartsWith(_data) ?? false;
        }
    }
}
