using SchedulerTelegramBot.Contracts.Constants;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Annotations;
using Telegrator.Annotations.StateKeeping;
using Telegrator.Handlers;
using Telegrator.StateKeeping;

namespace SchedulerTelegramBot.Bot.Handlers.Commands.BuySubscription
{
    [MessageHandler]
    [ChatType(ChatType.Private)]
    [EnumState<BuySubscriptionInputUserState>(BuySubscriptionInputUserState.WaitingForSubscriptionTime)]
    public class BuySubscriptionCommand_1(BuySubscriptionInfoStore infoStore): MessageHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            var message = container.HandlingUpdate.Message?.Text?.Replace("💰", "");

            if (message == null || !SubscriptionPricesConstants.SubscriptionPaymentTypes.TryGetValue(message, out var subscriptionType))
            {
                await Responce("Invalid subscription type", cancellationToken: cancellation, replyMarkup: new ReplyKeyboardRemove());
                return Result.Fault();
            }
            
            long chatId = container.ActualUpdate.Chat.Id;

            var storedData = infoStore.Get(chatId);

            storedData ??= new BuySubscriptionInfoStore.StoreData();

            storedData.SubscriptionType = message;

            infoStore.Set(chatId, storedData);

            var buttons = new List<InlineKeyboardButton[]> { };

            foreach (var apiName in PaymentApiConstants.PaymentApis)
            {
                buttons.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"{apiName}", $"buy-subscription-{apiName}") });
            }

            await Responce($"Good! You've selected {message}", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellation);
            await Responce("Choose your payment method", replyMarkup: buttons.ToArray(), cancellationToken: cancellation);

            container.ForwardEnumState<BuySubscriptionInputUserState>();

            return Result.Ok();
        }
    }
}
