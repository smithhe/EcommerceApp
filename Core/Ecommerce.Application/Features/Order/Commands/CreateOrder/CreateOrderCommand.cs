using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;
using MediatR;

namespace Ecommerce.Application.Features.Order.Commands.CreateOrder
{
	/// <summary>
	/// A <see cref="Mediator"/> request for creating a new <see cref="Order"/>
	/// </summary>
	public class CreateOrderCommand : IRequest<CreateOrderResponse>
	{
		/// <summary>
		/// The Order to be created
		/// </summary>
		public OrderDto? OrderToCreate { get; set; }
		
		/// <summary>
		/// The User requesting to create the Order
		/// </summary>
		public string? UserName { get; set; }
	}
}