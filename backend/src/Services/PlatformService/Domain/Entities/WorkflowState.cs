using PlatformService.Domain.Enums;

namespace PlatformService.Domain.Entities
{ 
public class WorkflowState
{
    public Guid InvoiceId { get; private set; }

    public string TrackingId { get; private set; } = string.Empty;

    public WorkflowStatus CurrentStatus { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public string? FailureReason { get; private set; }

    public WorkflowState(
        Guid invoiceId,
        string trackingId,
        DateTime createdAt)
    {
        InvoiceId = invoiceId;
        TrackingId = trackingId;

        CurrentStatus = WorkflowStatus.Submitted;

        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    private WorkflowState()
    {
    }

    public void MarkSubmitted(DateTime updatedAt)
    {
        CurrentStatus = WorkflowStatus.Submitted;
        UpdatedAt = updatedAt;
    }

    public void MarkApproved(DateTime updatedAt)
    {
        CurrentStatus = WorkflowStatus.Approved;
        UpdatedAt = updatedAt;
    }

    public void MarkPaymentRequested(DateTime updatedAt)
    {
        CurrentStatus = WorkflowStatus.PaymentRequested;
        UpdatedAt = updatedAt;
    }

    public void MarkPaid(DateTime updatedAt)
    {
        CurrentStatus = WorkflowStatus.Paid;
        UpdatedAt = updatedAt;
        FailureReason = null;
    }

    public void MarkPaymentFailed(
        DateTime updatedAt,
        string reason)
    {
        CurrentStatus = WorkflowStatus.PaymentFailed;
        UpdatedAt = updatedAt;
        FailureReason = reason;
    }
}
}