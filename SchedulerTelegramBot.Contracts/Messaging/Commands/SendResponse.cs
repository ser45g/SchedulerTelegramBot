namespace SchedulerTelegramBot.Contracts.Messaging.Commands
{
    public record class SendResponse(long ChatId, string Text);
}
