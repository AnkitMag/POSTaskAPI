using POSTaskAPI.Models;

namespace POSTaskAPI.DTO
{
    public class OrderRequest
    {
        public OrderType OrderType { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        // Relationship
        public List<OrderItem> OrderItems { get; set; }
    }
}
