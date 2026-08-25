using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
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
