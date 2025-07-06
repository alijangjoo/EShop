namespace EventBus.Messages.Events;

public class PaymentProcessedEvent : IntegrationBaseEvent
{
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int PaymentMethod { get; set; } // 1: IPG, 2: Cash
    public int Status { get; set; } // 1: Pending, 2: Processing, 3: Completed, 4: Failed, 5: Cancelled, 6: Refunded
    public string? TransactionId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? GatewayTransactionId { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? FailureReason { get; set; }
    public string? FailureReasonPersian { get; set; }
}