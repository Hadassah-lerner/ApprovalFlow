using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Application.Features.ProcessInvoice;
using ApprovalService.Application.Helpers;

namespace ApprovalService.Application.Nodes
{
    public class BuildPromptNode : IWorkflowNode
    {
        private readonly PromptBuilder _promptBuilder;

        public BuildPromptNode(PromptBuilder promptBuilder)
        {
            _promptBuilder = promptBuilder;
        }

        public async Task ExecuteAsync(WorkflowContext context)
        {
            context.FinalPrompt = _promptBuilder.BuildClassificationPrompt(
                context.ProcessedInvoiceText,
                context.PolicyText
            );

            await Task.CompletedTask;
        }
    }
}
