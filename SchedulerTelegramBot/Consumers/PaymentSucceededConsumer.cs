using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchedulerTelegramBot.Contracts;
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

            var subscription = await _dbContext.Subscription.FirstOrDefaultAsync(x => x.ChatId == chatId, context.CancellationToken);

            if(subscription != null)
            {
                //need optimistic concurrency (or an atomic db update)
                await _dbContext.Subscription.ExecuteUpdateAsync(setter => setter.SetProperty(x => x.EndsAtUtc, x => x.EndsAtUtc.AddMonths(12)), cancellationToken: context.CancellationToken);
            }
            else
            {
                _dbContext.Subscription.Add(new Subscription() { AddedAtUtc = DateTime.UtcNow, EndsAtUtc = DateTime.UtcNow.AddMonths(12), ChatId = chatId });

                await _dbContext.SaveChangesAsync(context.CancellationToken);
            }


            await _botClient.SendMessage(chatId: chatId,text: $"✅ Payment of {amount} RUB has been accepted!!!\nThank you for your purchase!",cancellationToken: context.CancellationToken);
           
        }
    }
}
