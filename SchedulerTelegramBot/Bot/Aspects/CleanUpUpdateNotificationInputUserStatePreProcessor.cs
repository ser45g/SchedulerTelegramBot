using SchedulerTelegramBot.Bot.Handlers.Commands.UpdateNotification;
using Telegrator;
using Telegrator.Aspects;
using Telegrator.Handlers.Components;
using Telegrator.StateKeeping;

namespace SchedulerTelegramBot.Bot.Aspects
{
    public class CleanUpUpdateNotificationInputUserStatePreProcessor : IPreProcessor
    {
        public Task<Result> BeforeExecution(IHandlerContainer container, CancellationToken cancellationToken = default)
        {

            var hasState = container.EnumStateKeeper<UpdateNotificationInputUserState>().HasState(container.HandlingUpdate);

            if (hasState && (container.EnumStateKeeper<UpdateNotificationInputUserState>().GetState(container.HandlingUpdate) != UpdateNotificationInputUserState.Start))
            {
                container.DeleteEnumState<UpdateNotificationInputUserState>();
            }

            return Task.FromResult(Result.Ok());
        }
    }
}
