using System.ComponentModel.DataAnnotations;

namespace SchedulerTelegramBot.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public required Guid ScheduledJobId { get; set; }

        public required long ChatId { get; set; }

        public required string Title { get; set; }

        public required DateTime AddedAtUtc { get; set; }

        public required DateTime NotifyAtUtc { get; set; }

        public string? Description { get; set; }

        public DateTime? LastUpdatedAtUtc { get; set; }

        public TimeSpan? PeriodicNotificationPeriod { get; set; }

        public uint RowVersion { get; set; }
    }
}
