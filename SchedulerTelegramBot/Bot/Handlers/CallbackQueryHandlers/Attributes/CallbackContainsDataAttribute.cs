using Telegram.Bot.Types;
using Telegrator.Annotations;
using Telegrator.Filters;
using Telegrator.Filters.Components;

namespace SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes
{
   
    public class CallbackContainsDataFilter : Filter<CallbackQuery>
    {
        private readonly string _data;

       
        public CallbackContainsDataFilter(string data)
        {
            _data = data;
        }

       
        public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
        {
            return context.Input.Data?.Contains(_data) ?? false;
        }
    }

    public class CallbackContainsDataAttribute(string data)
       : CallbackQueryAttribute(new CallbackContainsDataFilter(data))
    { }
}
