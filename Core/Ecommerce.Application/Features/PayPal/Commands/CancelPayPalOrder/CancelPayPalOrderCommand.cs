using MediatR;

namespace Ecommerce.Application.Features.PayPal.Commands.CancelPayPalOrder
{
    /// <summary>
    /// A <see cref="Mediator"/> request for cancelling an Ecommerce Order using PayPal as the payment method
    /// </summary>
    public class CancelPayPalOrderCommand : IRequest<bool>
    {
        /// <summary>
        /// The return key from the PayPal cancel return
        /// </summary>
        public string ReturnKey { get; set; } = null!;
    }
}