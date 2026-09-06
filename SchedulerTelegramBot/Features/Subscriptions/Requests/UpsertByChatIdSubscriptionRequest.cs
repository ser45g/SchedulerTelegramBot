
using MediatR;
using SchedulerTelegramBot.Features.Subscriptions.Responses;

namespace SchedulerTelegramBot.Features.Subscriptions.Requests
{
    public record class UpsertByChatIdSubscriptionRequest(long ChatId, TimeSpan AddTime) : IRequest<SubscriptionResponseDto>;

}
