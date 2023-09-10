using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Responses.Review;
using MediatR;

namespace Ecommerce.Application.Features.Review.Queries.GetUserReviewForProduct
{

	/// <summary>
	/// A <see cref="Mediator"/> request for retrieving a <see cref="EcommerceUser"/>'s <see cref="Review"/> of a <see cref="Product"/> if it exists
	/// </summary>
	public class GetUserReviewForProductQuery : IRequest<GetUserReviewForProductResponse>
	{
		/// <summary>
		/// The UserName of the <see cref="EcommerceUser"/>
		/// </summary>
		public string? UserName { get; set; }
		
		/// <summary>
		/// The unique identifier of the <see cref="Product"/> the <see cref="Review"/> is for
		/// </summary>
		public int ProductId { get; set; }
	}
}