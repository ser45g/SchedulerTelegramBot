using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Scalar.AspNetCore;
using SchedulerTelegramBot.Contracts.Payment;
using SchedulerTelegramBot.PaymentApi.Mediatr.Requests;
using SchedulerTelegramBot.PaymentApi.Options;

DotNetEnv.Env.Load(".env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddOptions<YoomoneyOptions>().BindConfiguration("YoomoneyOptions").ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapPost("/payment-link", async Task<Results<Ok<GetPaymentLinkRequest>, BadRequest>> (BuySubscriptionRequest req, ISender sender, CancellationToken cancellationToken) =>
{
    var link = await sender.Send(new BuySubscriptionWithYoomoneyRequest(req.Amount), cancellationToken);

    if (link == null)
        return TypedResults.BadRequest();

    return TypedResults.Ok(new GetPaymentLinkRequest(link));
});

app.MapPost("/success", (HttpContext context) =>
{
    Console.WriteLine("Success!");
});

app.Run();
