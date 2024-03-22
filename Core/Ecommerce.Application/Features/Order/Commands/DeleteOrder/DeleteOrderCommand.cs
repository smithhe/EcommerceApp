using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;
using MediatR;

namespace Ecommerce.Application.Features.Order.Commands.DeleteOrder
{
    /// <summary>
    /// A <see cref="Mediator"/> request for deleting an Ecommerce Order
    /// </summary>
    public class DeleteOrderCommand : IRequest<DeleteOrderResponse>
    {
        /// <summary>
        /// The Order to be deleted
        /// </summary>
        public OrderDto? Order { get; set; }
    }
}