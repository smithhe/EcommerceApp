using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Responses.Product;
using MediatR;

namespace Ecommerce.Application.Features.Product.Queries.GetProductsByCategoryId
{
	/// <summary>
	/// A <see cref="Mediator"/> request for retrieving all existing <see cref="Product"/> entities for an <see cref="Category"/>
	/// </summary>
	public class GetAllProductsByCategoryIdQuery : IRequest<GetAllProductsByCategoryIdResponse>
	{
		/// <summary>
		/// Id of the <see cref="Category"/> to find all <see cref="Product"/> entities for
		/// </summary>
		public int CategoryId { get; set; }
	}
}