using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Responses.OrderItem;
using MediatR;

namespace Ecommerce.Application.Features.OrderItem.Queries.GetAllOrderItemsByOrderId
{
	/// <summary>
	/// A <see cref="Mediator"/> request for retrieving all existing <see cref="OrderItem"/> entities for an <see cref="Order"/>
	/// </summary>
	public class GetAllOrderItemsByOrderIdQuery : IRequest<GetAllOrderItemsByOrderIdResponse>
	{
		/// <summary>
		/// Id of the <see cref="Order"/> to find all <see cref="OrderItem"/> entities for
		/// </summary>
		public int OrderId { get; set; }
	}
}