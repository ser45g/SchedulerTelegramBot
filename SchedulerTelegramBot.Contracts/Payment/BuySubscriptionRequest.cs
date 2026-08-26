namespace SchedulerTelegramBot.Contracts.Payment
{
    public record class BuySubscriptionRequest(long ChatId, decimal Amount);
}
