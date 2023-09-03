using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Responses.Product;
using MediatR;

namespace Ecommerce.Application.Features.Product.Queries.GetProductById
{
	/// <summary>
	/// A <see cref="Mediator"/> request for retrieving an existing <see cref="Product"/> by its Id
	/// </summary>
	public class GetProductByIdQuery : IRequest<GetProductByIdResponse>
	{
		/// <summary>
		/// The unique identifier of the <see cref="Product"/> to retrieve
		/// </summary>
		public int Id { get; set; }
	}
}