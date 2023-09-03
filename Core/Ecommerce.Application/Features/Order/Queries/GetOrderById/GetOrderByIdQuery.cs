using Ecommerce.Shared.Responses.Order;
using MediatR;

namespace Ecommerce.Application.Features.Order.Queries.GetOrderById
{
	/// <summary>
	/// A <see cref="Mediator"/> request for retrieving an existing <see cref="Order"/> by its Id
	/// </summary>
	public class GetOrderByIdQuery : IRequest<GetOrderByIdResponse>
	{
		/// <summary>
		/// The unique identifier of the <see cref="Order"/> to retrieve
		/// </summary>
		public int Id { get; set; }
	}
}