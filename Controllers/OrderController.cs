using ActiveX_Api.Constants;
using ActiveX_Api.Dto.Order;
using ActiveX_Api.Models;
using ActiveX_Api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActiveX_Api.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrder([FromRoute] int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            var order = await _context.Orders.Include(o => o.Address).Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.UserId != userId)
            {
                return Forbid();
            }

            return Ok(order);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateOrderDto orderDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.StripeSessionId == orderDto.StripeSessionId);
            if(order != null)
            {
                return BadRequest("Order already exists");
            }
            var address = await _context.Address.FindAsync(orderDto.AddressId);
            if(address == null)
            {
                return BadRequest("Address not found");
            }
            order = orderDto.FromCreateOrderDtoToOrder();
            order.UserId = userId;
            order.CreatedAt = DateTime.UtcNow;
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
    }
}
