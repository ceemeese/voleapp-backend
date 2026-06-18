namespace Application.Abstractions.Interfaces;

public record CheckoutSessionResult(string SessionId, string Url);

public interface IStripeService
{
    Task<CheckoutSessionResult> CreateCheckoutSessionAsync(decimal amount, int reservationId, string successUrl, string cancelUrl);
    Task<bool> VerifySessionSucceededAsync(string sessionId);
}
