using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSTaskAPI.DTO;
using POSTaskAPI.Models;
using POSTaskAPI.RepositoryInterface;

namespace POSTaskAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IGenericRepository<Order> orderRepo;
        private readonly IGenericRepository<OrderItem> orderItemRepo;
        private readonly IGenericRepository<Product> productRepo;

        public OrderController(IGenericRepository<Order> _orderRepo, IGenericRepository<OrderItem> _orderItemRepo, IGenericRepository<Product> productRepo)
        {
            orderRepo = _orderRepo;
            orderItemRepo = _orderItemRepo;
            this.productRepo = productRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await orderRepo.GetAllAsync(o => o.OrderItems);

            if (orders == null) return BadRequest("Orders not found");

            foreach (var orderdetails in orders.SelectMany(o => o.OrderItems))
            {
                orderdetails.Product = await productRepo.GetByIdAsync(orderdetails.ProductId);
            }

            return Ok(orders);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetOrder([FromRoute] int id)
        {
            var order = await orderRepo.GetByIdAsync(id);

            if (order != null)
            {
                return Ok(order);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(OrderRequest orderRequest)
        {
            try
            {
                if (orderRequest == null || !orderRequest.OrderItems.Any())
                    return BadRequest("Order must have at least one item.");

                // 1. Create the Order Header
                var order = new Order
                {
                    OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}", // Generates a unique number
                    OrderDate = DateTime.Now,
                    Type = orderRequest.OrderType,
                    //Type = Enum.TryParse<OrderType>(orderRequest.OrderType, out var orderType) ? orderType : OrderType.Delivery,
                    Status = orderRequest.Status,
                    TotalAmount = orderRequest.TotalAmount,
                    OrderItems = new List<OrderItem>()
                };

                decimal runningTotal = 0;


                foreach (var item in orderRequest.OrderItems)
                {
                    // Fetch the product from DB to get the official price
                    var product = await productRepo.GetByIdAsync(item.ProductId);
                    if (product == null)
                        return BadRequest($"Item with ID {item.ProductId} not found.");

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        Order = order,
                        Product = product
                    };

                    runningTotal += (product.Price * item.Quantity);
                    order.OrderItems.Add(orderItem);
                }

                // 3. Update final total and save
                order.TotalAmount = runningTotal;
                await orderRepo.AddAsync(order);

                return Ok(new
                {
                    Message = "Order placed successfully",
                    OrderNumber = order.OrderNumber,
                    Total = order.TotalAmount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            return Ok(await orderRepo.DeleteAsync(id));
        }
    }
}
