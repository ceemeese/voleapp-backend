using Application.Abstractions.Interfaces;
using Stripe.Checkout;

namespace Infrastructure.StripeService;

public class StripeService : IStripeService
{
    public async Task<CheckoutSessionResult> CreateCheckoutSessionAsync(decimal amount, int reservationId, string successUrl, string cancelUrl)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "eur",
                        UnitAmount = (long)(amount * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Reserva #{reservationId}",
                        }
                    },
                    Quantity = 1,
                }
            ],
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            Metadata = new Dictionary<string, string> { { "reservationId", reservationId.ToString() } }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return new CheckoutSessionResult(session.Id, session.Url);
    }

    public async Task<bool> VerifySessionSucceededAsync(string sessionId)
    {
        var service = new SessionService();
        var session = await service.GetAsync(sessionId);
        return session.PaymentStatus == "paid";
    }
}
