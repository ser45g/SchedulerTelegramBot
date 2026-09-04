using Telegrator.Annotations.StateKeeping;

namespace SchedulerTelegramBot.Bot.Handlers.Commands.UpdateNotification
{
    public enum InputUserState
    {
        Start = SpecialState.NoState,
        WaitingForTitle,
        WaitingForDescription,
        WaitingForNotificationDate
    }

}
