namespace YoomoneyInitializationUtils.Options
{
    public class YoomoneyOptions
    {
        public required string ClientId { get; init; }
        public required string RedirectUrl { get; init; }

        public string? ClientSecret { get; init; } = null;
        public string? Code { get; init; }
    }
}
