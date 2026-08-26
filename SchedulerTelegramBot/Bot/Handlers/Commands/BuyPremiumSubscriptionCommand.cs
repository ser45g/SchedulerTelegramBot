using Microsoft.Extensions.Configuration;
using SchedulerTelegramBot.Contracts.Payment;
using System.Net.Http.Json;
using Telegram.Bot.Types;
using Telegrator;
using Telegrator.Annotations;
using Telegrator.Handlers;

namespace SchedulerTelegramBot.Bot.Handlers.Commands
{
    [CommandHandler]
    [CommandAllias("buy")]
    public class BuyPremiumSubscriptionCommand(IConfiguration configuration, IHttpClientFactory httpClientFactory) : CommandHandler
    {

        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            using var httpClient = httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.PostAsJsonAsync("https://localhost:8081/payment-link", new BuySubscriptionRequest(container.ActualUpdate.Chat.Id, 2), cancellationToken: cancellation);
                
                if (response.IsSuccessStatusCode)
                {
                    var link = await response.Content.ReadFromJsonAsync<GetPaymentLinkRequest>(cancellationToken: cancellation);

                    if(link != null)
                    {
                        await container.Responce(link.Link, cancellationToken: cancellation);

                        return Result.Ok();
                    }
                }
            }
            catch (Exception ex) { 
                
            }
            await container.Responce("Could not recieve a payment link. Please, try again later.", cancellationToken: cancellation);

            return Result.Fault();
        }
    }
}
