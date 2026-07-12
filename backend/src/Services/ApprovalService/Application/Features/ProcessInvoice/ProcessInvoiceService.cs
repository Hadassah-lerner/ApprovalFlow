using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Application.Nodes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApprovalService.Application.Features.ProcessInvoice
{
    public class ProcessInvoiceService
    {
        private readonly List<IWorkflowNode> _pipeline;
        private readonly ILogger<ProcessInvoiceService> _logger;

        public ProcessInvoiceService(
                    PreprocessNode preprocessNode,
                    LoadPolicyNode loadPolicyNode,
                    BuildPromptNode buildPromptNode,
                    ClassifierNode classifierNode,
                    RouterNode routerNode,
                    HumanReviewNode humanReviewNode,
                    SaveNode saveNode,
                    ILogger<ProcessInvoiceService> logger)
        {
            _logger = logger;

            _pipeline = new List<IWorkflowNode>
            {
                preprocessNode,
                loadPolicyNode,
                buildPromptNode,
                classifierNode,
                routerNode,
                humanReviewNode,
                saveNode
            };
        }

        public async Task<WorkflowContext> ProcessAsync(object rawInvoice)
        {
            _logger.LogInformation("Starting End-to-End Invoice Process Pipeline...");

            var context = new WorkflowContext { Invoice = rawInvoice };

            try
            {
                foreach (var node in _pipeline)
                {
                    _logger.LogInformation($"Executing Node: {node.GetType().Name}");
                    await node.ExecuteAsync(context);

                    if (context.FinalDecision == "Rejected" && node.GetType().Name == "RouterNode")
                    {
                        _logger.LogWarning("Invoice rejected by RouterNode. Skipping HumanReview, jumping straight to Save.");
                    }
                }

                _logger.LogInformation($"Pipeline completed successfully. Final Decision: {context.FinalDecision}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical failure inside the ProcessInvoiceService Pipeline.");
                throw;
            }

            return context;
        }
    }
}
