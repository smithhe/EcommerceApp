using System;
using MediatR;

namespace Ecommerce.Application.Features.Order.Commands.AddPayPalRequestId
{
    /// <summary>
    /// A <see cref="Mediator"/> request for adding a PayPal Request Id to an <see cref="Order"/>
    /// </summary>
    public class AddPayPalRequestIdCommand : IRequest<bool>
    {
        /// <summary>
        /// The Id of the Order to add the PayPal Request Id to
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// The PayPal Request Id to add to the Order
        /// </summary>
        public Guid PayPalRequestId { get; set; }
    }
}