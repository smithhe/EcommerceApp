using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.OrderItem;
using MediatR;

namespace Ecommerce.Application.Features.OrderItem.Commands.CreateOrderItem
{
	/// <summary>
	/// A <see cref="Mediator"/> request for creating a new <see cref="OrderItem"/>
	/// </summary>
	public class CreateOrderItemCommand : IRequest<CreateOrderItemResponse>
	{
		public OrderItemDto OrderItemToCreate { get; set; } = null!;
	}
}