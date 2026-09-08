using MediatR;
using Microsoft.Extensions.Options;
using SchedulerTelegramBot.PaymentApi.Mediatr.Requests;
using SchedulerTelegramBot.PaymentApi.Options;
using YoomoneyApi.Quickpay;

namespace SchedulerTelegramBot.PaymentApi.Mediatr.Handlers
{
    public class BuySubscriptionWithYoomoneyHandler(IOptions<YooMoneyOptions> options) : IRequestHandler<BuySubscriptionWithYoomoneyRequest, string?>
    {
        public async Task<string?> Handle(BuySubscriptionWithYoomoneyRequest request, CancellationToken cancellationToken)
        {

            try
            {
                if(request.CurrencyCode != "RUB")
                {
                    throw new Exception("Only rubles are supported");
                }

                var quickpay = new Quickpay(receiver: options.Value.Reciever, quickpayForm: "shop", sum: request.Amount, label: $"({request.ChatId})({request.TimeSpan})", email: options.Value.Email, paymentType: "AC", firstname: "Sergey", lastname: "Alexashin", sender: request.ChatId.ToString()); 

                return quickpay.LinkPayment;

            }
            catch (Exception ex) 
            {
                return null;
            }
        }
    }
}
