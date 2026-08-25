using DotNetEnv.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using YoomoneyApi.Authorize;
using YoomoneyInitializationUtils.Options;

var envs = DotNetEnv.Env.Load(".env");

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices(services =>
{
    services.AddOptions<YoomoneyOptions>().BindConfiguration("YoomoneyOptions").Validate(options =>
    {
        bool result = true;

        result = result && options.ClientSecret != null; 
        result = result && options.ClientId != null; 
        result = result && options.RedirectUrl != null;
        return result;
    }).ValidateOnStart();
});

builder.ConfigureAppConfiguration(config =>
{
    config.AddJsonFile("appsettings.json");//goes first so it's overwritten
    config.AddDotNetEnv(".env", new DotNetEnv.LoadOptions(true));
});

var host = builder.Build();

using var scope = host.Services.CreateScope();

var options = scope.ServiceProvider.GetRequiredService<IOptions<YoomoneyOptions>>();

//it'll print a link to go to get code
var authorize = new Authorize(options.Value.ClientId, options.Value.RedirectUrl, [
    "account-info",
    "operation-history",
    "operation-details",
    "incoming-transfers",
    "payment-p2p"
]);

if(string.IsNullOrWhiteSpace(options.Value.Code))
{
    Console.WriteLine("Code can't be empty (it should be something like https://<domain>?code=<code>)");
    return;
}

var accessToken = await authorize.GetAccessToken(options.Value.Code, options.Value.ClientId, options.Value.RedirectUrl, options.Value.ClientSecret);

Console.WriteLine(string.IsNullOrWhiteSpace(accessToken)==false? $"Access Token: {accessToken}":"Something went wrong, couldn't get the access token");

host.Start();
