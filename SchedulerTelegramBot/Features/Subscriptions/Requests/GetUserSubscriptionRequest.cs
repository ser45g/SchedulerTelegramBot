
using MediatR;
using SchedulerTelegramBot.Features.Subscriptions.Responses;

public record class GetUserSubscriptionRequest(long ChatId) : IRequest<SubscriptionResponseDto>;
