using ActiveX_Api.Constants;
using ActiveX_Api.Dto.Address;
using ActiveX_Api.Mappers;
using ActiveX_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ActiveX_Api.Controllers
{
    [Route("api/address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApiUser> _userManager;

        public AddressController(AppDbContext context, UserManager<ApiUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAddress([FromRoute] int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;

            var address = await _context.Address.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            if (address.UserId != userId) return Unauthorized();

            return Ok(address);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateAddressDto addressDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;

            var address = addressDto.FromCreateAddresDtoToAddress();
            address.UserId = userId;

            await _context.Address.AddAsync(address);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAddress), new { id = address.Id }, address);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateAddressDto addressDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            var address = await _context.Address.FindAsync(id);
            if (address == null || address.IsDeleted)
            {
                return NotFound();
            }

            if (address.UserId != userId) return Unauthorized();
            address.FullName = addressDto.FullName;
            address.Street = addressDto.Street;
            address.Number = addressDto.Number;
            address.City = addressDto.City;
            address.State = addressDto.State;
            address.PostalCode = addressDto.PostalCode;
            address.Country = addressDto.Country;
            address.Phone = addressDto.Phone;
            await _context.SaveChangesAsync();
            return Ok(address);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            var address = await _context.Address.FindAsync(id);
            if (address == null || address.IsDeleted)
            {
                return NotFound();
            }
            if (address.UserId != userId) return Unauthorized();
            address.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
