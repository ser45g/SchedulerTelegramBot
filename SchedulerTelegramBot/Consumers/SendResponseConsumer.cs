using MassTransit;
using SchedulerTelegramBot.Contracts.Messaging.Commands;
using Telegram.Bot;

namespace SchedulerTelegramBot.Consumers
{
    public class SendResponseConsumer(ITelegramBotClient botClient) : IConsumer<SendResponse>
    {
        public async Task Consume(ConsumeContext<SendResponse> context)
        {
            await botClient.SendMessage(context.Message.ChatId, context.Message.Text, cancellationToken: context.CancellationToken);
        }
    }
}
