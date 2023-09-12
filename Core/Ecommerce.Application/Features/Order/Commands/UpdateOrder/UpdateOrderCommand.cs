using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;
using MediatR;

namespace Ecommerce.Application.Features.Order.Commands.UpdateOrder
{
	/// <summary>
	/// A <see cref="Mediator"/> request for updating an existing <see cref="Order"/>
	/// </summary>
	public class UpdateOrderCommand : IRequest<UpdateOrderResponse>
	{
		/// <summary>
		/// The Order to update with
		/// </summary>
		public OrderDto? OrderToUpdate { get; set; }
	}
}