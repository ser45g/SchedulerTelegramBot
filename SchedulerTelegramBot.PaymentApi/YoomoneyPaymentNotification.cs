using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace SchedulerTelegramBot.PaymentApi
{
    public record class YoomoneyPaymentNotification(
        [FromForm]string NotificationType,
        [FromForm]string OperationId,
        [FromForm]decimal Amount,
        [FromForm]int Currency,
        [FromForm]DateTime DateTime,
        [FromForm]string Sign,
        [FromForm]bool Unaccepted,
        [FromForm]decimal WithdrawAmount,
        [FromForm]string? Label = null,
        [FromForm]string? Sender = null,
        [FromForm]string? Email = null,
        [FromForm]string? Phone = null,
        [FromForm]string? LastName = null,
        [FromForm]string? FirstName = null,
        [FromForm]string? BillId=null,
        [FromForm]string? OperationLabel=null);
}
