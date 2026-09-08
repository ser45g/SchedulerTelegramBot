namespace SchedulerTelegramBot.Contracts.Constants
{
    public static class PaymentApiConstants
    {
        public readonly static SortedSet<string> PaymentApis = [YookassaApi, YoomoneyApi];

        public const string YoomoneyApi = "yoomoney";

        public const string YookassaApi = "yookassa";
    }
}
