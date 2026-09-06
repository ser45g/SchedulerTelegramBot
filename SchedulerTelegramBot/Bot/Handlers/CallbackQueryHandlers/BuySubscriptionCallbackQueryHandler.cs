using SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes;
using SchedulerTelegramBot.Bot.Handlers.Commands.BuySubscription;
using SchedulerTelegramBot.Contracts.Constants;
using SchedulerTelegramBot.Contracts.Http.Requests.Payment;
using SchedulerTelegramBot.Contracts.Http.Responses.Payment;
using System.Net.Http.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Handlers;
using Telegrator.StateKeeping;

namespace SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers
{
    [CallbackQueryHandler]
    [CallbackStartsWithData("buy-subscription")]
    public class BuySubscriptionCallbackQueryHandler(IHttpClientFactory httpClientFactory, BuySubscriptionInfoStore infoStore) : CallbackQueryHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<CallbackQuery> container, CancellationToken cancellationToken)
        {
            var callbackData = container.HandlingUpdate?.CallbackQuery?.Data;

            var chatId = container.HandlingUpdate?.GetChatId();

            if (chatId == null || callbackData == null || !PaymentApiConstants.PaymentApis.TryGetValue(callbackData.Replace("buy-subscription-", ""), out var paymentApi))
            {
                await container.Responce("Could not process the request. Please, try again later.", cancellationToken: cancellationToken);

                return Result.Fault();
            }

            try
            {
                var storedData = infoStore.Get(chatId.Value);

                ArgumentNullException.ThrowIfNull(storedData, nameof(storedData));
                ArgumentNullException.ThrowIfNull(storedData.SubscriptionType, nameof(storedData));

                if (!SubscriptionPricesConstants.SubscriptionPaymentTypes.TryGetValue(storedData.SubscriptionType, out var subscriptionPayment))
                {
                    throw new Exception("Couldn't get subscription type");
                }

                var link = await GetPaymentLink(chatId.Value, subscriptionPayment.Payment, subscriptionPayment.Currency, subscriptionPayment.TimeSpan, paymentApi, cancellationToken);

                if (link == null)
                {
                    await container.Responce("Could not recieve a payment link. Please, try again later.", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());

                    return Result.Fault();
                }

                await container.Responce($"Buy subscription ({subscriptionPayment.Name}) for {subscriptionPayment.Payment}{subscriptionPayment.Currency} for the current user using ({paymentApi})", replyMarkup: new InlineKeyboardButton("Follow", link), cancellationToken: cancellationToken);

                container.DeleteEnumState<BuySubscriptionInputUserState>();

                return Result.Ok();
            }
            catch (Exception ex) 
            {
                await container.Responce("Could not process the request. Please, try again later.", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());

                return Result.Fault();
            }
        }

        private async Task<string?> GetPaymentLink(long chatId, decimal amount, string currency, TimeSpan timeSpan, string apiName, CancellationToken cancellation=default)
        {
            using var httpClient = httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.PostAsJsonAsync($"https://localhost:8081/payment-link?apiName={apiName}", new   BuySubscriptionRequest(chatId, amount, currency, timeSpan), cancellationToken: cancellation);

                if (response.IsSuccessStatusCode)
                {
                    var link = await response.Content.ReadFromJsonAsync<PaymentLinkResponse>(cancellationToken: cancellation);

                    if (link != null)
                    {
                        return link.Link;
                    }
                }
            }
            catch (Exception)
            {

            }

            return null;
        }
    }
}
