using SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes;
using SchedulerTelegramBot.Contracts.Payment;
using SchedulerTelegramBot.Features.Notifications.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text;
using Telegram.Bot.Types;
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

            var chatId = container.HandlingUpdate?.CallbackQuery?.Message?.Chat.Id;

            if (chatId != null && callbackData != null && callbackData.StartsWith("buy-subscription-"))
            {
                var paymentApi = callbackData.Replace("buy-subscription-", "");

                if (string.Equals(paymentApi, "yoomoney", StringComparison.OrdinalIgnoreCase))
                {

                    var link = await GetPaymentLinkWithYoomoney(chatId.Value, 20, cancellationToken);

                    if (link != null)
                    {
                        await Responce(link, cancellationToken:cancellationToken);
                        return Result.Ok();
                    }

                    await container.Responce("Could not recieve a payment link. Please, try again later.", cancellationToken: cancellationToken);

                    return Result.Fault();


                }
                
            }

            return Result.Fault();
        }

        private async Task<string?> GetPaymentLinkWithYoomoney(long chatId, decimal amount, CancellationToken cancellation)
        {
            using var httpClient = httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.PostAsJsonAsync("https://localhost:8081/payment-link", new BuySubscriptionRequest(chatId, amount), cancellationToken: cancellation);

                if (response.IsSuccessStatusCode)
                {
                    var link = await response.Content.ReadFromJsonAsync<GetPaymentLinkRequest>(cancellationToken: cancellation);

                    if (link != null)
                    {
                        return link.Link;
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }
    }
}
