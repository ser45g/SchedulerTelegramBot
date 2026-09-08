using MediatR;
using Microsoft.EntityFrameworkCore;
using SchedulerTelegramBot.Data;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator;
using Telegrator.Annotations;
using Telegrator.Handlers;

namespace SchedulerTelegramBot.Bot.Handlers.Commands
{
    [CommandHandler]
    [CommandAllias("subscription_info")]
    public class PremiumCommand(ISender sender) : CommandHandler
    {
        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            long chatId = container.ActualUpdate.Chat.Id;

            var subscription = await sender.Send(new GetUserSubscriptionRequest(chatId), cancellation);

            if(subscription == null)
            {
                await Responce("There's no subscription. If you want to use this command, consider buying a subscription", cancellationToken: cancellation);

                return Result.Fault();
            }

            if (subscription.EndsAtUtc < DateTime.UtcNow)
            {
                await Responce("Your subscription is expired. If you want to use this command, consider prolonging your subscription", cancellationToken: cancellation);

                return Result.Fault();
            }

            await Responce($"You have {(subscription.EndsAtUtc - DateTime.UtcNow).Days} more days before your subscription expires", parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellation);

            return Result.Ok();
        }
    }
}
