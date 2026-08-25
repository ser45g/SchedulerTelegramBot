using Telegrator.Annotations.StateKeeping;

namespace SchedulerTelegramBot.Bot.Handlers.Commands.AddNotification
{
    public enum AddNotificationCommandInputUserState
    {
        Start = SpecialState.NoState,
        WaitingForTitle,
        WaitingForDescription,
        WaitingForNotificationDate
    }

}
