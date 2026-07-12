
namespace SubmissionService.Domain.Entities
{
    public class LineItem
    {
            public string Description { get; private set; } = string.Empty;
            public int Quantity { get; private set; }
            public decimal UnitPrice { get; private set; }
            public decimal Total => Quantity * UnitPrice;

        public LineItem(
         string description,
         int quantity,
         decimal unitPrice)
        {
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
        private LineItem()
        {
        }

    }
}
