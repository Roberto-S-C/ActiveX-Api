using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace ActiveX_Api.Controllers
{
    [Route("api/stripe")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        [Authorize]
        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetSessionDetails ([FromRoute] string sessionId)
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);
            if(session == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                session.Id,
                session.PaymentStatus,
                session.AmountTotal,
            });
        }
    }
}
