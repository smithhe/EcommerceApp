using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.PayPal;
using MediatR;

namespace Ecommerce.Application.Features.PayPal.Commands.CreatePayPalOrder
{
    /// <summary>
    /// A <see cref="Mediator"/> request for creating a new PayPal Order
    /// </summary>
    public class CreatePayPalOrderCommand : IRequest<CreatePayPalOrderResponse>
    {
        /// <summary>
        /// The internal Ecommerce order to create a PayPal Order for
        /// </summary>
        public OrderDto Order { get; set; } = null!;
    }
}