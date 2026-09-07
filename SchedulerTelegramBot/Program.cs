using Dorssel.EntityFrameworkCore;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using SchedulerTelegramBot.Consumers;
using SchedulerTelegramBot.Data;
using SchedulerTelegramBot.GlobalErrorHandlers;
using Telegrator;
using Telegrator.Hosting;

public partial class Program
{
    private static async Task Main(string[] args)
    {

        DotNetEnv.Env.Load(".env");

        var tgBuilder = TelegramBotHost.CreateBuilder(new TelegramBotHostBuilderSettings()
        {
            Args = args,
            ExceptIntersectingCommandAliases = true,
        });

        tgBuilder.Handlers.CollectHandlersAssemblyWide();
       
        var connectionString = tgBuilder.Configuration.GetConnectionString("DefaultConnection");

        if (connectionString == null)
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        tgBuilder.Services.AddDbContext<SchedulerDbContext>(options => options.UseSqlite(connectionString).UseSqliteTimestamp());

        tgBuilder.Services.AddSingleton<SchedulerTelegramBot.Bot.Handlers.Commands.AddNotification.AddNotificationInfoStore>();
        tgBuilder.Services.AddSingleton<SchedulerTelegramBot.Bot.Handlers.Commands.UpdateNotification.UpdateNotificationInfoStore>();
        tgBuilder.Services.AddSingleton<SchedulerTelegramBot.Bot.Handlers.Commands.BuySubscription.BuySubscriptionInfoStore>();

        tgBuilder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        tgBuilder.Services.AddQuartz();

        tgBuilder.Services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        tgBuilder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddEntityFrameworkOutbox<SchedulerDbContext>(options =>
            {
                options.UseSqlite();
                options.UseBusOutbox();

                options.QueryTimeout = TimeSpan.FromSeconds(3);
                options.QueryDelay = TimeSpan.FromSeconds(3);
            });


            x.AddRabbitMqConfigureEndpointsCallback((context, name, cfg) =>
            {
                cfg.UseQueueBasedDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));
                cfg.UseEntityFrameworkOutbox<SchedulerDbContext>(context, options =>
                {
                    options.ConcurrentDeliveryLimit = 10;
                });
                cfg.UseMessageRetry(r => r.Immediate(3).Incremental(3, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200)));
            });

            x.AddConsumer<PaymentSucceededConsumer>();
            x.AddConsumer<SendResponseConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("amqp://localhost:5672", h =>
                {
                    h.Username("rabbitmq");
                    h.Password("rabbitmq");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        TelegramBotHost telegramBot = tgBuilder.Build();

        telegramBot.UpdateRouter.ExceptionHandler = new GlobalExcepitonHandler(telegramBot.Services.GetService<ILogger<GlobalExcepitonHandler>>()!);
        
        telegramBot.SetBotCommands();
        
        using (var context = telegramBot.Services.GetRequiredService<SchedulerDbContext>())
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            await context.Database.MigrateAsync();
        }

        await telegramBot.RunAsync();
    }
}