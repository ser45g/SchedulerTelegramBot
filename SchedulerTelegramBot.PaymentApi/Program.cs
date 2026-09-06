using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using SchedulerTelegramBot.Contracts.Http.Requests.Payment;
using SchedulerTelegramBot.Contracts.Http.Responses.Payment;
using SchedulerTelegramBot.Contracts.Messaging.Events;
using SchedulerTelegramBot.PaymentApi.Mediatr.Requests;
using SchedulerTelegramBot.PaymentApi.Options;
using System.Text.RegularExpressions;
using YooKassaNet.Webhooks;

DotNetEnv.Env.Load(".env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddOptions<YooMoneyOptions>().BindConfiguration("YooMoneyOptions").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<YooKassaOptions>().BindConfiguration("YooKassaOptions").ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddOpenApi();

builder.Services.AddHttpClient();

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

app.MapPost("/payment-link", async Task<Results<Ok<PaymentLinkResponse>, BadRequest>> (BuySubscriptionRequest req, [FromQuery] string apiName, ISender sender, CancellationToken cancellationToken) =>
{
    IRequest<string?> request = apiName switch
    {
        "yoomoney" => new BuySubscriptionWithYoomoneyRequest(req.ChatId, req.Amount, req.CurrencyCode, req.TimeSpan),
        "yookassa" => new BuySubscriptionWithYookassaRequest(req.ChatId, req.Amount, req.CurrencyCode, req.TimeSpan),
        _ => new BuySubscriptionWithYookassaRequest(req.ChatId, req.Amount, req.CurrencyCode, req.TimeSpan)
    };

    var link = await sender.Send(request, cancellationToken);

    if (link == null)
        return TypedResults.BadRequest();

    return TypedResults.Ok(new PaymentLinkResponse(link));
});

app.MapPost("/notification/yoomoney", async( 
    IPublishEndpoint publish,
    HttpContext context,
    CancellationToken cancellationToken,
    [FromForm] decimal amount,
    [FromForm] long currency,
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

    var pattern = @"\(([^)]+)\)\(([^)]+)\)";

    if (string.IsNullOrWhiteSpace(label))
        throw new ArgumentNullException(nameof(label));

    var match = Regex.Match(label, pattern);

    if (!match.Success)
        throw new FormatException("Input string is not in the expected format");

    long chatId = long.Parse(match.Groups[1].Value);

    TimeSpan timeSpan = TimeSpan.Parse(match.Groups[2].Value);

    await publish.Publish(new PaymentSucceededEvent(chatId, timeSpan, amount, "RUB", DateTime.UtcNow));
       
}).DisableAntiforgery();

app.MapPost("/success", () =>
{
    return Results.Ok();
});

app.MapPost("/notification/yookassa", async (HttpContext httpContext, IPublishEndpoint publishEndpoint, CancellationToken cancellationToken=default) =>
{
    var streamReader = new StreamReader(httpContext.Request.Body);

    var notificationString = await streamReader.ReadToEndAsync(cancellationToken);

    var notification = YooKassaNotification.Parse(notificationString);

    switch (notification.Event)
    {
        case WebhookEvent.PaymentSucceeded:
            var paid = notification.AsPayment();

            if(paid.Metadata != null)
            {
                var isChatIdPresent = paid.Metadata.TryGetValue("chat_id", out var chatIdString);
                var isTimeSpanPresent = paid.Metadata.TryGetValue("time_span", out var timeSpanString);

                if (isChatIdPresent==true && isTimeSpanPresent && long.TryParse(chatIdString, out long chatId) && TimeSpan.TryParse(timeSpanString, out var timeSpan))
                {
                    await publishEndpoint.Publish(new PaymentSucceededEvent(chatId, timeSpan, paid.Amount.Value, paid.Amount.Currency.ToString(), paid.CreatedAt.DateTime), cancellationToken);
                }
            }

            break;
    }
    
    return Results.Ok();
});

app.Run();
