using MassTransit;
using MassTransit.Transports;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchedulerTelegramBot.Contracts.Messaging.Commands;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Entities;
using SchedulerTelegramBot.Features.Subscriptions.Requests;
using SchedulerTelegramBot.Features.Subscriptions.Responses;
using SchedulerTelegramBot.Mappers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SchedulerTelegramBot.Features.Subscriptions.Handlers
{
    public class UpsertByChatIdSubscriptionHandler(SchedulerDbContext context, ITelegramBotClient botClient, ISendEndpointProvider sendEndpointProvider) : IRequestHandler<UpsertByChatIdSubscriptionRequest, SubscriptionResponseDto?>
    {
        public async Task<SubscriptionResponseDto?> Handle(UpsertByChatIdSubscriptionRequest request, CancellationToken cancellationToken)
        {
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:send-response"));

            var subscription = await context.Subscription.FirstOrDefaultAsync(x=>x.ChatId == request.ChatId, cancellationToken);

            if (subscription == null)
            {
                subscription = new Subscription() { AddedAtUtc = DateTime.UtcNow, ChatId = request.ChatId, EndsAtUtc = DateTime.UtcNow.Add(request.AddTime), LastUpdatedAtUtc = null };

                context.Subscription.Add(subscription);
            }
            else
            {
                subscription.EndsAtUtc = subscription.EndsAtUtc > DateTime.UtcNow? subscription.EndsAtUtc.Add(request.AddTime): DateTime.UtcNow.Add(request.AddTime);

                subscription.LastUpdatedAtUtc = DateTime.UtcNow;

                context.Subscription.Update(subscription);
            }

            await sendEndpoint.Send(new SendResponse(request.ChatId, $"✅ Payment has been accepted!!! Now you have {(subscription.EndsAtUtc - DateTime.UtcNow).Days} more days.\nThank you for your purchase!"), cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return subscription.ToSubscriptionResponseDto();
        }
    }
}
