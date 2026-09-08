namespace SchedulerTelegramBot.Contracts.Constants
{
    public static class SubscriptionPricesConstants
    {

        public record class SubscriptionPayment(string Name, decimal Payment, string Currency, TimeSpan TimeSpan);

        public readonly static List<SubscriptionPayment> _subscriptionTypes = new List<SubscriptionPayment>() {
            new SubscriptionPayment("1 week", 27m, "RUB", TimeSpan.FromDays(7)),
            new SubscriptionPayment("1 month", 100m, "RUB", TimeSpan.FromDays(30)),
            new SubscriptionPayment("2 months", 190m, "RUB", TimeSpan.FromDays(60)),
            new SubscriptionPayment("3 months", 275m, "RUB", TimeSpan.FromDays(90)),
            new SubscriptionPayment("6 months", 520m, "RUB", TimeSpan.FromDays(180))
        };

        public readonly static IDictionary<string, SubscriptionPayment> SubscriptionPaymentTypes = _subscriptionTypes.ToDictionary(x => $"{x.Name} - {x.Payment}{x.Currency}", x => x);

    }
}
