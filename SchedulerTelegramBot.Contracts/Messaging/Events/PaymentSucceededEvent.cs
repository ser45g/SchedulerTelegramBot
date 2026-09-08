namespace SchedulerTelegramBot.Contracts.Messaging.Events
{
    public record class PaymentSucceededEvent(long ChatId, TimeSpan TimeSpan, decimal Amount, string CurrencyCode, DateTime OccuredAtUtc);
}
