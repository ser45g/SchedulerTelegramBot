using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.MadiatorCore;

namespace SchedulerTelegramBot.GlobalErrorHandlers
{
    public class GlobalExcepitonHandler : IRouterExceptionHandler
    {
        private readonly ILogger<GlobalExcepitonHandler> _logger;

        public GlobalExcepitonHandler(ILogger<GlobalExcepitonHandler> logger)
        {
            _logger = logger;
        }

        public void HandleException(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            _logger.LogError(exception , "Error occurred in {Source}", source);            
        }
    }
}
