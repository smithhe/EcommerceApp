using MediatR;

namespace Ecommerce.Application.Features.PayPal.Commands.CreatePayPalReturnKey
{
    /// <summary>
    /// A <see cref="Mediator"/> request for creating a return key for a PayPal order
    /// </summary>
    public class CreatePayPalReturnKeyCommand : IRequest<string?>
    {
        /// <summary>
        /// The Id of the Order to create the return key for
        /// </summary>
        public int OrderId { get; set; }
    }
}