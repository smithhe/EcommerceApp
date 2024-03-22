using Ecommerce.Shared.Dtos;
using MediatR;

namespace Ecommerce.Application.Features.PayPal.Queries.GetOrderByReturnKey
{
    /// <summary>
    /// A <see cref="Mediator"/> request for getting an Ecommerce Order by the PayPal return key
    /// </summary>
    public class GetOrderByReturnKeyQuery : IRequest<OrderDto?>
    {
        /// <summary>
        /// The return key to find the Ecommerce Order
        /// </summary>
        public string ReturnKey { get; set; } = null!;
    }
}