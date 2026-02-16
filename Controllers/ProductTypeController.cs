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
    public class ProductTypeController : Controller
    {
        private readonly IGenericRepository<ProductType> productTypeRepo;

        public ProductTypeController(IGenericRepository<ProductType> _productTypeRepo)
        {
            productTypeRepo = _productTypeRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductTypes()
        {
            var producttypes = await productTypeRepo.GetAllAsync(pt => pt.Items);
            return Ok(producttypes);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetProductType([FromRoute] int id)
        {
            var productType = await productTypeRepo.GetByIdAsync(id);

            if (productType != null)
            {
                return Ok(productType);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddProductType(ProductTypeRequest productTypeRequest)
        {
            try
            {
                bool typeExist = await productTypeRepo.AnyAsync("Name", productTypeRequest.Name);
                if (typeExist)
                {
                    return BadRequest("Category already exist.");
                }

                var productType = new ProductType()
                {
                    Name = productTypeRequest.Name
                };

                await productTypeRepo.AddAsync(productType);

                return Ok(productType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateProducts([FromRoute] int id, ProductTypeRequest updateRequest)
        {
            var productType = await productTypeRepo.GetByIdAsync(id);

            if (productType != null)
            {

                productType.Name = updateRequest.Name;
                await productTypeRepo.UpdateAsync(productType, id);
                return Ok(productType);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            return Ok(await productTypeRepo.DeleteAsync(id));
        }
    }
}
