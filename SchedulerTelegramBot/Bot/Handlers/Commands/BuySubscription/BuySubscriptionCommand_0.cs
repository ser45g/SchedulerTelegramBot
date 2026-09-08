using SchedulerTelegramBot.Bot.Aspects;
using SchedulerTelegramBot.Bot.Handlers.Commands.AddNotification;
using SchedulerTelegramBot.Contracts.Constants;
using Telegram.Bot.Types;
using Telegrator;
using Telegrator.Annotations;
using Telegrator.Aspects;
using Telegrator.Handlers;
using Telegrator.StateKeeping;

namespace SchedulerTelegramBot.Bot.Handlers.Commands.BuySubscription
{
    [CommandHandler]
    [BeforeExecution<CleanUpBuySubscriptionInputUserStatePreProcessor>()]
    [CommandAllias("buy")]
    public class BuySubscriptionCommand_0 : CommandHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            var paymentTypes = new List<string> { };

            foreach (var type in SubscriptionPricesConstants.SubscriptionPaymentTypes)
            {
                paymentTypes.Add($"💰{type.Key}");
            }

            await Responce("Choose the period", replyMarkup: paymentTypes.Chunk(3).ToArray(), cancellationToken: cancellation);

            container.ForwardEnumState<BuySubscriptionInputUserState>();

            return Result.Ok();
        }
    }
}
