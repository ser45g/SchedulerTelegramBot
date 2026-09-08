using SchedulerTelegramBot.Entities;
using SchedulerTelegramBot.Features.Subscriptions.Responses;

namespace SchedulerTelegramBot.Mappers
{
    public static class SubscriptionMappers
    {
        public static SubscriptionResponseDto ToSubscriptionResponseDto(this Subscription subscription)
        {
            return new SubscriptionResponseDto(subscription.Id, subscription.ChatId, subscription.AddedAtUtc, subscription.EndsAtUtc, subscription.RowVersion, subscription.LastUpdatedAtUtc);
        }
    }
}
