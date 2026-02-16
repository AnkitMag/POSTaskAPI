using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSTaskAPI.Models;
using POSTaskAPI.RepositoryInterface;

namespace POSTaskAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IGenericRepository<Product> productRepo;
        private readonly IGenericRepository<ProductType> productTypeRepo;

        public ProductController(IGenericRepository<Product> _productRepo, IGenericRepository<ProductType> _productTypeRepo)
        {
            productRepo = _productRepo;
            productTypeRepo = _productTypeRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(await productRepo.GetAllAsync());
        }

        [HttpGet("type")]
        public async Task<IActionResult> GetProductTypes()
        {
            return Ok(await productTypeRepo.GetAllAsync());
        }

        //[HttpGet("bytype")]
        //[Route("{typeId:int}")]
        //public async Task<IActionResult> GetProductByTypeId([FromRoute] int typeId)
        //{
        //    var products = await productRepo.GetAllAsync();
        //    var productsById = products.Where(x => x.CategoryId == typeId);

        //    if (productsById != null)
        //    {
        //        return Ok(productsById);
        //    }

        //    return NotFound();
        //}

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            var product = await productRepo.GetByIdAsync(id);

            if (product != null)
            {
                return Ok(product);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductRequest productRequest)
        {
            try
            {
                bool categoryExists = await productTypeRepo.AnyAsync("Name", productRequest.CategoryName);
                if (!categoryExists)
                {
                    return BadRequest("Invalid Category ID.");
                }

                var productType = await productTypeRepo.GetByValueAsync("Name", productRequest.CategoryName);
                var product = new Product()
                {
                    Name = productRequest.Name,
                    Price = productRequest.Price,
                    CategoryId = productType.Id
                };

                await productRepo.AddAsync(product);

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateProducts([FromRoute] int id, ProductRequest updateRequest)
        {
            var product = await productRepo.GetByIdAsync(id);

            if (product != null)
            {
                bool categoryExists = await productTypeRepo.AnyAsync("Name", updateRequest.CategoryName);
                if (!categoryExists)
                {
                    return BadRequest("Invalid Category ID.");
                }

                var productType = await productTypeRepo.GetByValueAsync("Name", updateRequest.CategoryName);

                product.Name = updateRequest.Name;
                product.Price = updateRequest.Price;
                product.CategoryId = productType.Id;
                await productRepo.UpdateAsync(product, id);
                return Ok(product);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            return Ok(await productRepo.DeleteAsync(id));
        }
    }
}
