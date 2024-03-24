using Ecommerce.Shared.Responses.Order;
using MediatR;

namespace Ecommerce.Application.Features.Order.Queries.GetOrderAfterSuccessfulCheckout
{
    /// <summary>
    /// A <see cref="Mediator"/> request to get an Order after a successful checkout
    /// </summary>
    public class GetOrderAfterSuccessfulCheckoutQuery : IRequest<GetOrderAfterSuccessfulCheckoutResponse>
    {
        /// <summary>
        /// The unique identifier of the Order to retrieve
        /// </summary>
        public int Id { get; set; }
    }
}