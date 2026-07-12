using ApprovalService.Domain.Models;
namespace ApprovalService.Domain.Interfaces
{
        public interface IOllamaClassifierService
        {
            Task<ClassificationResult> ClassifyInvoiceAsync(string vendor, decimal total, string itemsDescription);
        }

}