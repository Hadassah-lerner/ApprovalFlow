using ApprovalService.Application.Features.ProcessInvoice;

namespace ApprovalService.Application.Common.Abstractions
{
    public interface IWorkflowNode
    {
        Task ExecuteAsync(WorkflowContext context);
    }
}
