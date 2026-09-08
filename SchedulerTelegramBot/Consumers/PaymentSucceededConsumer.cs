using MassTransit;
using MediatR;
using SchedulerTelegramBot.Contracts.Messaging.Events;
using SchedulerTelegramBot.Features.Subscriptions.Requests;

namespace SchedulerTelegramBot.Consumers
{
    public class PaymentSucceededConsumer : IConsumer<PaymentSucceededEvent>
    {
        private readonly ISender sender;

        public PaymentSucceededConsumer(ISender sender)
        {
            this.sender = sender;
        }

        public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
        {
            var chatId = context.Message.ChatId;

            var amount = context.Message.Amount;

            var timeSpan = context.Message.TimeSpan;

            await sender.Send(new UpsertByChatIdSubscriptionRequest(chatId, timeSpan));
        }
    }
}
