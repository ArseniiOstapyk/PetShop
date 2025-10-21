using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.DTO.Order;
using PetShop.Interfaces;
using System.Security.Claims;

namespace PetShop.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;

        public OrdersController(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) throw new UnauthorizedAccessException("User ID not found in token.");
            return int.Parse(claim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            int userId = GetUserId();
            var list = await _orderRepo.GetMyOrdersAsync(userId);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMyOrderById(int id)
        {
            int userId = GetUserId();
            var dto = await _orderRepo.GetMyOrderByIdAsync(id, userId);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddListingToCurrentOrNewOrder([FromBody] AddSingleListingDto request)
        {
            int userId = GetUserId();
            var dto = await _orderRepo.AddListingToCurrentOrNewOrderAsync(userId, request.ListingId);
            return Ok(dto);
        }

        [HttpDelete("{orderId}/items/{orderListingId}")]
        public async Task<IActionResult> RemoveItem(int orderId, int orderListingId)
        {
            int userId = GetUserId();
            var ok = await _orderRepo.RemoveItemAsync(orderId, orderListingId, userId);
            if (!ok) return Forbid("You can remove items only from your own orders.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetUserId();
            var ok = await _orderRepo.DeleteAsync(id, userId);
            if (!ok) return Forbid("You can delete only your own orders.");
            return NoContent();
        }

        [HttpPost("{id}/pay")]
        public async Task<IActionResult> MarkPaid(int id)
        {
            int userId = GetUserId();
            var ok = await _orderRepo.MarkPaidAsync(id, userId);
            if (!ok) return Forbid("You can only mark your own orders as paid.");
            return NoContent();
        }
    }
}
