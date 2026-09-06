using MediatR;

namespace SchedulerTelegramBot.PaymentApi.Mediatr.Requests
{
    public record class BuySubscriptionWithYoomoneyRequest(long ChatId, decimal Amount, string CurrencyCode, TimeSpan TimeSpan) : IRequest<string?>;
}
