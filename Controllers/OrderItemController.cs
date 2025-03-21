using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ActiveX_Api.Models;
using ActiveX_Api.Dto.OrderItem;
using ActiveX_Api.Mappers;
using Microsoft.AspNetCore.Authorization;

namespace ActiveX_Api.Controllers
{
    [Route("api/orderItem")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderItemController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrderItem([FromRoute] int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            return Ok(orderItem);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateOrderItemDto orderItemDto)
        {
            var order = await _context.Orders.FindAsync(orderItemDto.OrderId);
            var product = await _context.Products.FindAsync(orderItemDto.ProductId);
            if(order == null)
            {
                return BadRequest("Order not found");
            }
            if (product == null)
            {
                return BadRequest("Product not found");
            }
            var orderItem = orderItemDto.FromCreateOrderItemDtoOrderItem(); 
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrderItem), new { id = orderItem.Id }, orderItem.FromOrderItemToGetOrderItemDto());
        }
    }
}
