using MediatR;
using Microsoft.EntityFrameworkCore;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Entities;
using SchedulerTelegramBot.Features.Subscriptions.Requests;
using SchedulerTelegramBot.Features.Subscriptions.Responses;
using SchedulerTelegramBot.Mappers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SchedulerTelegramBot.Features.Subscriptions.Handlers
{
    public class UpsertByChatIdSubscriptionHandler(SchedulerDbContext context, ITelegramBotClient botClient) : IRequestHandler<UpsertByChatIdSubscriptionRequest, SubscriptionResponseDto?>
    {
        public async Task<SubscriptionResponseDto?> Handle(UpsertByChatIdSubscriptionRequest request, CancellationToken cancellationToken)
        {
            var subscription = await context.Subscription.FirstOrDefaultAsync(x=>x.ChatId == request.ChatId, cancellationToken);

            if (subscription == null)
            {
                subscription = new Subscription() { AddedAtUtc = DateTime.UtcNow, ChatId = request.ChatId, EndsAtUtc = DateTime.UtcNow.Add(request.AddTime), LastUpdatedAtUtc = null };
            }
            else
            {
                subscription.EndsAtUtc = subscription.EndsAtUtc > DateTime.UtcNow? subscription.EndsAtUtc.Add(request.AddTime): DateTime.UtcNow.Add(request.AddTime);

                subscription.LastUpdatedAtUtc = DateTime.UtcNow;

                context.Subscription.Update(subscription);
            }

            await botClient.SendMessage(chatId: request.ChatId, text: $"✅ Payment has been accepted!!! Now you have {(subscription.EndsAtUtc - DateTime.UtcNow).Days} more days.\nThank you for your purchase!", cancellationToken: cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return subscription.ToSubscriptionResponseDto();
        }
    }
}
