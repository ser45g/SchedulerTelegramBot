namespace SchedulerTelegramBot.Contracts
{
    public record class PaymentSucceededEvent(long ChatId, decimal Amount, DateTime OccuredAtUtc);
}
