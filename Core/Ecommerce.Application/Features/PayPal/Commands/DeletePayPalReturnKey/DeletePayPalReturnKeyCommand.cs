using MediatR;

namespace Ecommerce.Application.Features.PayPal.Commands.DeletePayPalReturnKey
{
    /// <summary>
    /// A <see cref="Mediator"/> request for deleting a PayPal return key
    /// </summary>
    public class DeletePayPalReturnKeyCommand : IRequest<bool>
    {
        /// <summary>
        /// The return key to delete
        /// </summary>
        public string? ReturnKey { get; set; }
    }
}