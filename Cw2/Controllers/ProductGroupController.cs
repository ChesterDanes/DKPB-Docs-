using BLL.DTOModels.RequestsDTO;
using BLL.ServiceInterfaces.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw2.Controllers
{
    [Route("api/productGroups")]
    [ApiController]
    public class ProductGroupController : ControllerBase
    {
        private readonly IProductGroupService _productGroupService;

        public ProductGroupController(IProductGroupService productGroupService)
        {
            _productGroupService = productGroupService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProductGroup([FromBody] ProductGroupRequestDTO groupRequest)
        {
            if (groupRequest == null || string.IsNullOrWhiteSpace(groupRequest.Name))
            {
                return BadRequest("Nieprawidłowe dane grupy produktów.");
            }

            var result = await _productGroupService.AddProductGroupAsync(groupRequest);

            return CreatedAtAction(nameof(AddProductGroup), new { id = result.ID }, result);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetProductGroups(
        [FromQuery] bool topGroups = false,
        [FromQuery] int? groupId = null,
        [FromQuery] string sortBy = "Name",
        [FromQuery] bool ascending = true)
        {
            var groups = await _productGroupService.GetProductGroupsAsync(topGroups, groupId, sortBy, ascending);

            if (groups == null || !groups.Any())
            {
                return NotFound("Nie znaleziono grup produktów.");
            }

            return Ok(groups);
        }
    }

}
