using System.Text.Json.Serialization;

namespace POSTaskAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        [JsonIgnore]
        public ProductType Category { get; set; }
    }

    public class ProductType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Product> Items { get; set; }
    }
}
