using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using SchedulerTelegramBot.Contracts;
using SchedulerTelegramBot.Contracts.Payment;
using SchedulerTelegramBot.PaymentApi;
using SchedulerTelegramBot.PaymentApi.Mediatr.Requests;
using SchedulerTelegramBot.PaymentApi.Options;
using System.Reflection;
using System.Reflection.Emit;
using System.Web;
using YoomoneyApi.Account;
using YoomoneyApi.Operation;

DotNetEnv.Env.Load(".env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddOptions<YoomoneyOptions>().BindConfiguration("YoomoneyOptions").ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddOpenApi();

builder.Services.AddMassTransit(configure =>
{
    configure.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("amqp://localhost:5672", h =>
        {
            h.Username("rabbitmq");
            h.Password("rabbitmq");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();



app.MapPost("/payment-link", async Task<Results<Ok<GetPaymentLinkRequest>, BadRequest>> (BuySubscriptionRequest req, ISender sender, CancellationToken cancellationToken) =>
{
    var link = await sender.Send(new BuySubscriptionWithYoomoneyRequest(req.ChatId, req.Amount), cancellationToken);

    if (link == null)
        return TypedResults.BadRequest();

    return TypedResults.Ok(new GetPaymentLinkRequest(link));
});

app.MapPost("/success", async( 
    IPublishEndpoint publish,
    HttpContext context,
    CancellationToken cancellationToken,
    [FromForm] decimal amount,
    [FromForm] int currency,
    [FromForm] DateTime dateTime,
    [FromForm] string sign,
    [FromForm] string? operationId=null,
    [FromForm] string? notificationType=null,
    [FromForm] bool? unaccepted = null,
    [FromForm] decimal? withdrawAmount = null, 
    [FromForm] string? label = null,
    [FromForm] string? sender = null,
    [FromForm] string? email = null,
    [FromForm] string? phone = null,
    [FromForm] string? lastName = null,
    [FromForm] string? firstName = null,
    [FromForm] string? billId = null,
    [FromForm] string? operationLabel = null) =>
{
    if(long.TryParse(label, out long chatId))
    {
        await publish.Publish(new PaymentSucceededEvent(chatId, amount, DateTime.UtcNow));
        return;
    }
    throw new Exception("Could not parse the sender");

}).DisableAntiforgery();

app.Run();
