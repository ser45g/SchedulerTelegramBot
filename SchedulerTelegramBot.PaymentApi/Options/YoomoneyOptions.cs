using System.ComponentModel.DataAnnotations;

namespace SchedulerTelegramBot.PaymentApi.Options
{
    public class YooMoneyOptions
    {
        [Required]
        public required string AccessToken { get; init; }
        [Required]
        public required string Reciever { get; init; }
        [Required]
        public required string Email { get; init; }
    }
}
