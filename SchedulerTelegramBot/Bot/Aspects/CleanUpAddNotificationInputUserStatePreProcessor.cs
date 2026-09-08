using SchedulerTelegramBot.Bot.Handlers.Commands.AddNotification;
using Telegrator;
using Telegrator.Aspects;
using Telegrator.Handlers.Components;
using Telegrator.StateKeeping;

namespace SchedulerTelegramBot.Bot.Aspects
{
    public class CleanUpAddNotificationInputUserStatePreProcessor : IPreProcessor
    {
        public Task<Result> BeforeExecution(IHandlerContainer container, CancellationToken cancellationToken = default)
        {

            var hasState = container.EnumStateKeeper<AddNotificationInputUserState>().HasState(container.HandlingUpdate);

            if (hasState && (container.EnumStateKeeper<AddNotificationInputUserState>().GetState(container.HandlingUpdate) != AddNotificationInputUserState.Start))
            {
                container.DeleteEnumState<AddNotificationInputUserState>();
            }

            return Task.FromResult(Result.Ok());
        }
    }
}
