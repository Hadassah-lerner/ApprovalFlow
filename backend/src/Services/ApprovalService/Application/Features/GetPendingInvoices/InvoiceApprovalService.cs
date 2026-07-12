using ApprovalService.Application.Interfaces;
using ApprovalService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApprovalService.Application.Features.GetPendingInvoices
{
    public class InvoiceApprovalService
    {
        private readonly IInvoiceRepository _repository;

        public InvoiceApprovalService(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<InvoiceApproval>> GetPendingInvoicesAsync(string status)
        {
            return await _repository.GetByStatusAsync(status);
        }
    }
}