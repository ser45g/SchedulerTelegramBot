using MediatR;

namespace SchedulerTelegramBot.PaymentApi.Mediatr.Requests
{
    public record class BuySubscriptionWithYoomoneyRequest(decimal Amount) : IRequest<string?>;
}
