using MediatR;
using Microsoft.Extensions.Options;
using SchedulerTelegramBot.PaymentApi.Mediatr.Requests;
using SchedulerTelegramBot.PaymentApi.Options;
using YooKassaNet;
using YooKassaNet.Payments;

namespace SchedulerTelegramBot.PaymentApi.Mediatr.Handlers
{
    public class BuySubscriptionWithYookassaHandler(IOptions<YooKassaOptions> options, IHttpClientFactory httpClientFactory) : IRequestHandler<BuySubscriptionWithYookassaRequest, string?>
    {
        public async Task<string?> Handle(BuySubscriptionWithYookassaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                using var client = httpClientFactory.CreateClient();

                var payments = new YooKassaPaymentsClient(client, new YooKassaClientOptions()
                {
                    ShopId = options.Value.ShopId,
                    SecretKey = options.Value.SecretKey,
                });

                if (request.CurrencyCode != "RUB")
                    throw new Exception("Only rubles are supported");

                // Создаем одностадийный платеж и отправляем покупателя на страницу подтверждения.
                var payment = await payments.CreatePaymentAsync(new CreatePaymentRequest
                {
                    Amount = Money.Rubles(request.Amount),
                    Capture = true,
                    Confirmation = Confirmation.Redirect("https://t.me/frantic_beaver_bot"),
                    Description = "Buying the subscription for a year",
                    Metadata = new Dictionary<string, string>() { 
                        ["chat_id"] = request.ChatId.ToString(),
                        ["time_span"] = request.TimeSpan.ToString(),
                    }
                }, cancellationToken: cancellationToken);

                return payment.Confirmation?.ConfirmationUrl;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
