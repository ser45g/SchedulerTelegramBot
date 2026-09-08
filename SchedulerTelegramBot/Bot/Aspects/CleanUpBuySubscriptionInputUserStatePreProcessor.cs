using SchedulerTelegramBot.Bot.Handlers.Commands.BuySubscription;
using Telegrator;
using Telegrator.Aspects;
using Telegrator.Handlers.Components;
using Telegrator.StateKeeping;

namespace SchedulerTelegramBot.Bot.Aspects
{
    public class CleanUpBuySubscriptionInputUserStatePreProcessor : IPreProcessor
    {
        public Task<Result> BeforeExecution(IHandlerContainer container, CancellationToken cancellationToken = default)
        {

            var hasState = container.EnumStateKeeper<BuySubscriptionInputUserState>().HasState(container.HandlingUpdate);

            if (hasState && (container.EnumStateKeeper<BuySubscriptionInputUserState>().GetState(container.HandlingUpdate) != BuySubscriptionInputUserState.Start))
            {
                container.DeleteEnumState<BuySubscriptionInputUserState>();
            }

            return Task.FromResult(Result.Ok());
        }
    }
}
