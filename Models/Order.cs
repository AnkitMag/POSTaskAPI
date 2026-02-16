using System.Text.Json.Serialization;

namespace POSTaskAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderType Type { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        // Relationship
        public ICollection<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Product Product { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }
    }

    public enum OrderType
    {
        Delivery,
        DineIn,
        TakeAway
    }
}
