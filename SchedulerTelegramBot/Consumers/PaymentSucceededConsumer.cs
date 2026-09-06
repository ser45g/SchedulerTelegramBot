using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchedulerTelegramBot.Contracts.Constants;
using SchedulerTelegramBot.Contracts.Messaging.Events;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.Entities;
using Telegram.Bot;

namespace SchedulerTelegramBot.Consumers
{
    public class PaymentSucceededConsumer : IConsumer<PaymentSucceededEvent>
    {
        private readonly ITelegramBotClient _botClient;
        private readonly SchedulerDbContext _dbContext;
        private readonly ILogger<PaymentSucceededConsumer> _logger;

        public PaymentSucceededConsumer(
            ITelegramBotClient botClient,
            SchedulerDbContext dbContext,
            ILogger<PaymentSucceededConsumer> logger)
        {
            _botClient = botClient;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
        {
            var chatId = context.Message.ChatId;
            var amount = context.Message.Amount;

            var timeSpan = context.Message.TimeSpan;
            
            var subscription = await _dbContext.Subscription.FirstOrDefaultAsync(x => x.ChatId == chatId, context.CancellationToken);

            if(subscription != null)
            {
                //need optimistic concurrency (or an atomic db update)
                subscription.EndsAtUtc = subscription.EndsAtUtc.Add(timeSpan);

                _dbContext.Subscription.Update(subscription);
            }
            else
            {
                _dbContext.Subscription.Add(new Subscription() { AddedAtUtc = DateTime.UtcNow, EndsAtUtc = DateTime.UtcNow.Add(timeSpan), ChatId = chatId });

            }
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            await _botClient.SendMessage(chatId: chatId, text: $"✅ Payment of {amount} RUB has been accepted!!!\nThank you for your purchase!",cancellationToken: context.CancellationToken);
           
        }
    }
}
