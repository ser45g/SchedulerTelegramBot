namespace SchedulerTelegramBot.Features.Subscriptions.Responses
{
    public record class SubscriptionResponseDto(Guid Id, long ChatId, DateTime AddedAtUtc, DateTime EndsAtUtc, uint RowVersion, DateTime? LastUpdatedAtUtc=null);
}