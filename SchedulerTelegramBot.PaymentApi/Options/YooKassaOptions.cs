using System.ComponentModel.DataAnnotations;

namespace SchedulerTelegramBot.PaymentApi.Options
{
    public class YooKassaOptions
    {
        [Required]
        public required string ShopId { get; init; }

        [Required]
        public required string SecretKey { get; init; }
    }
}
