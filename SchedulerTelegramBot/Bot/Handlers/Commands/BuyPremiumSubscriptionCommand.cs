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
            long chatId = container.ActualUpdate.Chat.Id;

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
