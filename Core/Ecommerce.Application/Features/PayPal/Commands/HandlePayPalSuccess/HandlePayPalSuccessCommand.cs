using MediatR;

namespace Ecommerce.Application.Features.PayPal.Commands.HandlePayPalSuccess
{
    /// <summary>
    /// A <see cref="Mediator"/> request for handling a successful return from PayPal
    /// </summary>
    public class HandlePayPalSuccessCommand : IRequest<bool>
    {
        /// <summary>
        /// The return key from the PayPal success return
        /// </summary>
        public string? ReturnKey { get; set; }
    }
}