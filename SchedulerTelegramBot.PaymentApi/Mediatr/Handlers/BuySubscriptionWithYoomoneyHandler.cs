using MediatR;
using Microsoft.Extensions.Options;
using SchedulerTelegramBot.PaymentApi.Mediatr.Requests;
using SchedulerTelegramBot.PaymentApi.Options;
using YoomoneyApi.Account;
using YoomoneyApi.Quickpay;

namespace SchedulerTelegramBot.PaymentApi.Mediatr.Handlers
{
    public class BuySubscriptionWithYoomoneyHandler(IOptions<YoomoneyOptions> options) : IRequestHandler<BuySubscriptionWithYoomoneyRequest, string?>
    {
        public async Task<string?> Handle(BuySubscriptionWithYoomoneyRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var client = new Client(token: options.Value.AccessToken);

                //Payment method. Possible values: PC - payment from the YuMoney wallet; AC - from a bank card.
                var quickpay = new Quickpay(receiver: options.Value.Reciever, quickpayForm: "shop", sum: request.Amount, label: request.ChatId.ToString(), email: options.Value.Email, paymentType: "AC", firstname: "Sergey", lastname: "Alexashin", sender: request.ChatId.ToString()); 

                return quickpay.LinkPayment;

            }
            catch (Exception ex) 
            {
                return null;
            }
        }
    }
}
