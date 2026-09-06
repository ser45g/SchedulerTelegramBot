namespace SchedulerTelegramBot.Contracts.Http.Requests.Payment
{
    public record class BuySubscriptionRequest(long ChatId, decimal Amount, string CurrencyCode, TimeSpan TimeSpan);
}
