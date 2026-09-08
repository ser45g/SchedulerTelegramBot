using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;
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

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x=>x.Title).IsRequired().HasMaxLength(300);
                entity.Property(x=>x.Description).HasMaxLength(800);
                entity.Property(x => x.AddedAtUtc).IsRequired();
                entity.Property(x => x.LastUpdatedAtUtc);
                entity.Property(x => x.ScheduledJobId).IsRequired();
                entity.Property(x => x.NotifyAtUtc).IsRequired();
                entity.Property(x => x.RowVersion).IsRowVersion();

                entity.HasIndex(x=>x.ChatId);
            });

            modelBuilder.Entity<Subscription>(entity => 
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.AddedAtUtc).IsRequired();
                entity.Property(x => x.EndsAtUtc).IsRequired();
                entity.Property(x => x.LastUpdatedAtUtc);
                entity.Property(x => x.ChatId).IsRequired();
                entity.Property(x=>x.RowVersion).IsRowVersion();

                entity.HasIndex(x => x.ChatId).IsUnique();
            });

            modelBuilder.AddTransactionalOutboxEntities();
            modelBuilder.AddQuartz(o => o.UsePostgreSql());
        }

    }
}
