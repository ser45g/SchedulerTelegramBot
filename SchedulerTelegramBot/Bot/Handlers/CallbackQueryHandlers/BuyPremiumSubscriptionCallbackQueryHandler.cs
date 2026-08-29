using SchedulerTelegramBot.Bot.Handlers.CallbackQueryHandlers.Attributes;
using SchedulerTelegramBot.Contracts.Payment;
using SchedulerTelegramBot.Features.Notifications.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text;
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

            var chatId = container.HandlingUpdate?.CallbackQuery?.Message?.Chat.Id;

            if (chatId != null && callbackData != null && callbackData.StartsWith("buy-subscription-"))
            {
                var paymentApi = callbackData.Replace("buy-subscription-", "");

                var link = await GetPaymentLink(chatId.Value, 20, paymentApi, cancellationToken);

                if (link != null)
                {
                    await container.Responce("Купить подписку на один год для данного пользователя", replyMarkup: new InlineKeyboardButton("Перейти", link), cancellationToken: cancellationToken);
                    return Result.Ok();
                }

                await container.Responce("Could not recieve a payment link. Please, try again later.", cancellationToken: cancellationToken);

                return Result.Fault();

            }

            return Result.Fault();
        }

        private async Task<string?> GetPaymentLink(long chatId, decimal amount, string apiName, CancellationToken cancellation)
        {
            using var httpClient = httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.PostAsJsonAsync($"https://localhost:8081/payment-link?apiName={apiName}", new BuySubscriptionRequest(chatId, amount), cancellationToken: cancellation);

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
