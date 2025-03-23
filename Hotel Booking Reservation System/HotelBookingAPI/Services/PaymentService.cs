using Stripe;
using Stripe.Checkout;

namespace HotelBookingAPI.Services
{
    public class PaymentService
    {
        private readonly IConfiguration _configuration;

        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<string> CreateCheckoutSession(decimal amount, string currency)
        {
            Console.WriteLine($"➡️ Creating checkout session for amount: {amount}, currency: {currency}");

            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.");
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = currency,
                    UnitAmount = (long)(amount * 100), //  Convert to smallest currency unit
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Hotel Booking"
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = "http://localhost:5173/payment/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "http://localhost:5173/payment/cancel"
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            Console.WriteLine($" Checkout URL: {session.Url}");

            return session.Url;
        }

    }
}
