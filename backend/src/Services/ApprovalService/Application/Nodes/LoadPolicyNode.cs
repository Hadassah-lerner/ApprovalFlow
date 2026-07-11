using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Application.Features.ProcessInvoice;

namespace ApprovalService.Application.Nodes
{
    public class LoadPolicyNode : IWorkflowNode
    {
        private readonly IPolicyLoader _policyLoader;

        public LoadPolicyNode(IPolicyLoader policyLoader)
        {
            _policyLoader = policyLoader;
        }

        public async Task ExecuteAsync(WorkflowContext context)
        {
            context.PolicyText = _policyLoader.LoadPolicy();
            await Task.CompletedTask;
        }
    }
}