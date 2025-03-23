using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Mvc;
using HotelBookingAPI.Models;
using Stripe.Checkout;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] PaymentRequest request)
        {
            Console.WriteLine($"🔥 Received amount: {request.Amount}"); //  Log the received amount

            if (request.Amount <= 0)
            {
                return BadRequest("Invalid amount");
            }

            var checkoutUrl = await _paymentService.CreateCheckoutSession(request.Amount, request.Currency);
            return Ok(new { url = checkoutUrl });
        }

        [HttpGet("verify-session")]
        public async Task<IActionResult> VerifySession(string sessionId)
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            if (session.PaymentStatus == "paid")
            {
                return Ok(new { message = "Payment successful", sessionId = sessionId });
            }

            return BadRequest(new { message = "Payment not completed" });
        }


        [HttpGet("success")]
        public IActionResult PaymentSuccess([FromQuery] string session_id)
        {
            return Ok(new { Message = "Payment successful", SessionId = session_id });
        }

        [HttpGet("cancel")]
        public IActionResult PaymentCancel()
        {
            return BadRequest(new { Message = "Payment canceled" });
        }
    }
}
