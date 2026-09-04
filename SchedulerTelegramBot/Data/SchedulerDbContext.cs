using MassTransit;
using Microsoft.EntityFrameworkCore;
using SchedulerTelegramBot.Entities;


namespace SchedulerTelegramBot.Data
{
    public class SchedulerDbContext : DbContext
    {
        public SchedulerDbContext(DbContextOptions<SchedulerDbContext> options): base(options) {}

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Subscription> Subscription{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddTransactionalOutboxEntities();
        }

    }
}
