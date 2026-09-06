using MediatR;
using Microsoft.EntityFrameworkCore;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Features.Subscriptions.Responses;
using SchedulerTelegramBot.Mappers;

namespace SchedulerTelegramBot.Features.Subscriptions.Handlers
{
    public class GetSubscriptionHandler(SchedulerDbContext context) : IRequestHandler<GetUserSubscriptionRequest, SubscriptionResponseDto?>
    {
        public async Task<SubscriptionResponseDto?> Handle(GetUserSubscriptionRequest request, CancellationToken cancellationToken)
        {
            var subscription = await context.Subscription.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

            if (subscription == null)
            {
                return null;
            }
            return subscription.ToSubscriptionResponseDto();
        }
    }
}
