using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Responses.Review;
using MediatR;

namespace Ecommerce.Application.Features.Review.Queries.GetReviewsForProduct
{
	/// <summary>
	/// A <see cref="Mediator"/> request for retrieving all existing <see cref="Review"/> entities for an <see cref="Product"/>
	/// </summary>
	public class GetReviewsForProductQuery : IRequest<GetReviewsForProductResponse>
	{
		/// <summary>
		/// Id of the <see cref="Product"/> to find all <see cref="Review"/> entities for
		/// </summary>
		public int ProductId { get; set; }
	}
}