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
    public class PremiumCommand : CommandHandler
    {
        private readonly SchedulerDbContext _dbContext;

        public PremiumCommand(SchedulerDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public override async Task<Result> Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            long chatId = container.ActualUpdate.Chat.Id;

            var subscription = await _dbContext.Subscription.FirstOrDefaultAsync(x => x.ChatId == chatId, cancellation);

            if(subscription == null)
            {
                await Responce("There's no subscription. If you want to use this command, consider buying a subscription");
                return Result.Fault();
            }

            if (subscription.EndsAtUtc < DateTime.UtcNow)
            {
                await Responce("Your subscription is expired. If you want to use this command, consider prolonging your subscription");
                return Result.Fault();
            }

            await Responce($"Your subscription ends at: {subscription.EndsAtUtc}", parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellation);

            return Result.Ok();
        }
    }
}
