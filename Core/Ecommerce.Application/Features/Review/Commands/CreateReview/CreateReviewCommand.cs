using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Review;
using MediatR;

namespace Ecommerce.Application.Features.Review.Commands.CreateReview
{
	/// <summary>
	/// A <see cref="Mediator"/> request for creating a new <see cref="Review"/>
	/// </summary>
	public class CreateReviewCommand : IRequest<CreateReviewResponse>
	{
		/// <summary>
		/// The <see cref="Review"/> to be created
		/// </summary>
		public ReviewDto? ReviewToCreate { get; set; }
	}
}