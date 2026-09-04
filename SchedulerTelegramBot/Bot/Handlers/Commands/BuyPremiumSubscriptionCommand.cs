using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Annotations;
using Telegrator.Handlers;

namespace SchedulerTelegramBot.Bot.Handlers.Commands
{
    [CommandHandler]
    [CommandAllias("buy")]
    public class BuyPremiumSubscriptionCommand : CommandHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            List<string> paymentApiNames = ["yoomoney", "yookassa"];

            var buttons = new List<InlineKeyboardButton[]> { };

            foreach (var apiName in paymentApiNames)
            {
                buttons.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"{apiName}", $"buy-subscription-{apiName}") });
            }

            await Responce("Choose your payment method", replyMarkup: buttons.ToArray(), cancellationToken: cancellation);

            return Result.Ok();
        }
    }
}
