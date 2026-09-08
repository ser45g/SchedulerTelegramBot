using Telegrator.Annotations.StateKeeping;

namespace SchedulerTelegramBot.Bot.Handlers.Commands.BuySubscription
{
    public enum BuySubscriptionInputUserState
    {
        Start = SpecialState.NoState,
        WaitingForSubscriptionTime,
        WaitingForPaymentApiName,
    }

}
