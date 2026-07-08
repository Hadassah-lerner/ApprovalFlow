
namespace ApprovalService.Domain.Entities
{
    public class LineItemApproval
    {
        public Guid Id { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Total => Quantity * UnitPrice;

        public LineItemApproval(
         string description,
         int quantity,
         decimal unitPrice)
        {
            Id = Guid.NewGuid();
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
        private LineItemApproval()
        {
        }

    }
}
