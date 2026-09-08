using System.ComponentModel.DataAnnotations;

namespace SchedulerTelegramBot.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }

        public required long ChatId { get; set; }

        public required DateTime AddedAtUtc { get; set; }

        public required DateTime EndsAtUtc { get; set; }

        public DateTime? LastUpdatedAtUtc { get; set; }

        public uint RowVersion { get; set; }
    }
}
