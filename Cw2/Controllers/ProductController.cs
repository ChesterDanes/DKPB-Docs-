using BLL.DTOModels.ResponseDTO;
using BLL.DTOModels;
using BLL.ServiceInterfaces.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw2.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductResponseDTO>>> GetProducts(
            [FromQuery] string? nameFilter,
            [FromQuery] string? groupNameFilter,
            [FromQuery] int? groupIdFilter,
            [FromQuery] bool includeInactive = false,
            [FromQuery] string? sortBy = "Name",
            [FromQuery] bool ascending = true)
        {
            
            var products = await _productService.GetProductsAsync(nameFilter, groupNameFilter, groupIdFilter, includeInactive, sortBy, ascending);
            return Ok(products);
        }

        [HttpPut("{id}/deactivate")]
        public async Task DeactivateProduct(int id)
        {
            await _productService.DeactivateProductAsync(id);
        }

        [HttpPut("{id}/activate")]
        public async Task ActivateProduct(int id)
        {
            await _productService.ActivateProductAsync(id);
            
        }

        [HttpDelete("{id}")]
        public async Task DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponseDTO>> AddProduct([FromBody] ProductRequestDTO productRequest)
        {
            if (productRequest == null)
            {
                return BadRequest("Niepoprawne dane produktu.");
            }

            await _productService.AddProductAsync(productRequest);

            return Ok();
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromQuery] int productId, [FromQuery] int userId, [FromQuery] int amount)
        {
            if (productId <= 0 || userId <= 0 || amount <= 0)
            {
                return BadRequest("Invalid input parameters.");
            }

            try
            {
                // Wywołanie metody z serwisu do dodania produktu do koszyka
                await _productService.AddToCartAsync(productId, userId, amount);
                return Ok("Product added to cart.");
            }
            catch (Exception ex)
            {
                // Logowanie błędu i zwrócenie odpowiedzi w przypadku wyjątku
                // Możesz dodać logowanie błędów (np. Log.Error(ex.Message)) w zależności od swoich potrzeb
                return StatusCode(500, "An error occurred while adding the product to the cart.");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProductAmountInCart([FromQuery] int productId, [FromQuery] int userId, [FromQuery] int amount)
        {
            if (productId <= 0 || userId <= 0 || amount < 0)
            {
                return BadRequest("Invalid input parameters.");
            }

            try
            {
                // Wywołanie metody z serwisu do aktualizacji ilości produktu w koszyku
                await _productService.UpdateProductAmountInCartAsync(productId, userId, amount);
                return Ok("Product amount updated successfully.");
            }
            catch (Exception ex)
            {
                // Logowanie błędu i zwrócenie odpowiedzi w przypadku wyjątku
                return StatusCode(500, "An error occurred while updating the product amount in the cart.");
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromCart([FromQuery] int productId, [FromQuery] int userId)
        {
            if (productId <= 0 || userId <= 0)
            {
                return BadRequest("Invalid productId or userId.");
            }

            try
            {
                // Wywołanie metody z serwisu do usunięcia produktu z koszyka
                await _productService.RemoveFromCartAsync(productId, userId);
                return Ok("Product removed from cart successfully.");
            }
            catch (Exception ex)
            {
                // Logowanie błędu i zwrócenie odpowiedzi w przypadku wyjątku
                return StatusCode(500, "An error occurred while removing the product from the cart.");
            }
        }
    }
}
