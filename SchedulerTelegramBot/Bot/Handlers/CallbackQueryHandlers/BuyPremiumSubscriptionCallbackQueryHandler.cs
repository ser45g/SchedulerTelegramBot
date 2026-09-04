using SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes;
using SchedulerTelegramBot.Contracts.Payment;
using System.Net.Http.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Handlers;

namespace SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers
{
    [CallbackQueryHandler]
    [CallbackContainsData("buy-subscription")]
    public class BuyPremiumSubscriptionCallbackQueryHandler(IHttpClientFactory httpClientFactory) : CallbackQueryHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<CallbackQuery> container, CancellationToken cancellationToken)
        {
            var callbackData = container.HandlingUpdate?.CallbackQuery?.Data;

            var chatId = container.HandlingUpdate?.GetChatId();

            if (chatId == null || callbackData == null || !callbackData.StartsWith("buy-subscription-"))
            {
                await container.Responce("Could not process the request. Please, try again later.", cancellationToken: cancellationToken);

                return Result.Fault();
            }

            string paymentApi = callbackData.Replace("buy-subscription-", "");

            var link = await GetPaymentLink(chatId.Value, 20, paymentApi, cancellationToken);

            if (link == null)
            {
                await container.Responce("Could not recieve a payment link. Please, try again later.", cancellationToken: cancellationToken);

                return Result.Fault();
            }

            await container.Responce("Buy subscription for a year for the current user", replyMarkup: new InlineKeyboardButton("Follow", link), cancellationToken: cancellationToken);

            return Result.Ok();
        }

        private async Task<string?> GetPaymentLink(long chatId, decimal amount, string apiName, CancellationToken cancellation=default)
        {
            using var httpClient = httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.PostAsJsonAsync($"https://localhost:8081/payment-link?apiName={apiName}", new   BuySubscriptionRequest(chatId, amount), cancellationToken: cancellation);

                if (response.IsSuccessStatusCode)
                {
                    var link = await response.Content.ReadFromJsonAsync<GetPaymentLinkRequest>(cancellationToken: cancellation);

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
