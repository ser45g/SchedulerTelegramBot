using Telegrator.Annotations.StateKeeping;

namespace SchedulerTelegramBot.Bot.Handlers.Commands.AddNotification
{
    public enum AddNotificationInputUserState
    {
        Start = SpecialState.NoState,
        WaitingForTitle,
        WaitingForDescription,
        WaitingForNotificationDate
    }

}
