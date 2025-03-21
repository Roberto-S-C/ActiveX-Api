using ActiveX_Api.Constants;
using ActiveX_Api.Dto.Checkout;
using ActiveX_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;

namespace ActiveX_Api.Controllers
{
    [Route("create-checkout-session")]
    [ApiController]
    public class CheckoutApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CheckoutApiController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] List<Checkout> products)
        {
            var domain = Urls.Frontend;
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{domain}/checkout/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{domain}/checkout/canceled"
            };

            foreach (var product in products)
            {
                var itemOptions = new SessionLineItemOptions
                {
                    Price = product.StripePriceId,
                    Quantity = product.Quantity
                };
                options.LineItems.Add(itemOptions);
            }

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Ok(new { url = session.Url });
        }
    }
}
