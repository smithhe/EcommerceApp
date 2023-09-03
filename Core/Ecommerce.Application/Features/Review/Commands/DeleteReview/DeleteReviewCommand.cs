using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Review;
using MediatR;

namespace Ecommerce.Application.Features.Review.Commands.DeleteReview
{
	/// <summary>
	/// A <see cref="Mediator"/> request for deleting a <see cref="Review"/>
	/// </summary>
	public class DeleteReviewCommand : IRequest<DeleteReviewResponse>
	{
		/// <summary>
		/// The <see cref="Review"/> to delete
		/// </summary>
		public ReviewDto? ReviewToDelete { get; set; }
	}
}