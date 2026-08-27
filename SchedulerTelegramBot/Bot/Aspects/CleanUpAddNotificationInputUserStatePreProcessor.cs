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

            var hasState = container.EnumStateKeeper<InputUserState>().HasState(container.HandlingUpdate);

            if (hasState && (container.EnumStateKeeper<InputUserState>().GetState(container.HandlingUpdate) != InputUserState.Start))
            {
                container.DeleteEnumState<InputUserState>();
            }

            return Task.FromResult(Result.Ok());
        }
    }
}
