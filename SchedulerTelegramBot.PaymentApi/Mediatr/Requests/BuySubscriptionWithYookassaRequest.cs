using MediatR;

namespace SchedulerTelegramBot.PaymentApi.Mediatr.Requests
{
    public record class BuySubscriptionWithYookassaRequest(long ChatId, decimal Amount) : IRequest<string?>;
}
