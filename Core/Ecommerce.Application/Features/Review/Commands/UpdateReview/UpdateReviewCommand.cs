using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Review;
using MediatR;

namespace Ecommerce.Application.Features.Review.Commands.UpdateReview
{
	/// <summary>
	/// A <see cref="Mediator"/> request for updating an existing <see cref="Review"/>
	/// </summary>
	public class UpdateReviewCommand : IRequest<UpdateReviewResponse>
	{
		/// <summary>
		/// The <see cref="Review"/> to update with
		/// </summary>
		public ReviewDto? ReviewToUpdate { get; set; }
	}
}