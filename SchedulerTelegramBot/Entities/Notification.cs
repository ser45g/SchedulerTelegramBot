using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerTelegramBot.Entities
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        public required long ChatId { get; set; }

        public required string Title { get; set; }

        public required DateTime AddedDateTime { get; set; }

        public required DateTime NotifyDateTime { get; set; }

        public string? Description { get; set; }
        public DateTime? LastUpdatedDateTime { get; set; }

        public TimeSpan? PeriodicNotificationPeriod { get; set; }
    }
}
