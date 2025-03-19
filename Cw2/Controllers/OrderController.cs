using BLL.DTOModels.ResponseDTO;
using BLL.ServiceInterfaces.Interfaces;
using BLL_EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw2.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetOrders(
        [FromQuery] string orderIdFilter = null,
        [FromQuery] bool? isPaidFilter = null,
        [FromQuery] string sortBy = "ID",
        [FromQuery] bool ascending = true)
        {
            try
            {
                var orders = await _orderService.GetOrdersAsync(orderIdFilter, isPaidFilter, sortBy, ascending);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                // Log the exception (could be done with a logger here)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching orders.");
            }
        }

        [HttpPost("generate")]
        public async Task<ActionResult<OrderResponseDTO>> GenerateOrder([FromQuery] int userId)
        {
            var order = await _orderService.GenerateOrderAsync(userId);

            if (order == null)
            {
                return BadRequest("Brak użytkownika lub produktów w koszyku.");
            }

            return CreatedAtAction(nameof(GenerateOrder), new { id = order.ID }, order);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderPositionResponseDTO>> GetOrderPosition(int orderId)
        {
            var orderPosition = await _orderService.GetOrderPositionAsync(orderId);

            if (orderPosition == null)
            {
                return NotFound("Nie znaleziono pozycji zamówienia.");
            }

            return Ok(orderPosition);
        }

        [HttpPost("pay/{orderId}")]
        public async Task<IActionResult> PayOrder(int orderId, [FromQuery] string amountPaid)
        {
            decimal amount = Convert.ToDecimal(amountPaid);
            var paymentResult = await _orderService.PayOrderAsync(orderId, amount);

            return paymentResult? Ok("Zamówienie zostało opłacone.") : Ok("Błąd płatności. Sprawdź szczegóły zamówienia");
        }
    }
}
