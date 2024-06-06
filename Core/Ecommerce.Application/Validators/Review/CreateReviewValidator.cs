using Ecommerce.Application.Features.Review.Commands.CreateReview;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Product.Queries.GetProductById;
using MediatR;

namespace Ecommerce.Application.Validators.Review
{
	public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
	{
		private readonly IReviewAsyncRepository _reviewAsyncRepository;
		private readonly IMediator _mediator;

		public CreateReviewValidator(IReviewAsyncRepository reviewAsyncRepository, IMediator mediator)
		{
			this._reviewAsyncRepository = reviewAsyncRepository;
			this._mediator = mediator;

			RuleFor(c => c.ReviewToCreate!)
				.MustAsync(ReviewDoesNotExist).WithMessage("Review already exists for this product");
			
			RuleFor(c => c)
				.MustAsync(ProductExists).WithMessage("Product must exist");
			
			//TODO: Check that user exists

			RuleFor(c => c.ReviewToCreate!.Stars)
				.GreaterThanOrEqualTo(0).WithMessage("Number of stars must be greater than or equal to 0");

			RuleFor(c => c.ReviewToCreate!.Comments)
				.Must(SafeCommentCheck).WithMessage("Comment must not exceed 500 characters");
		}

		private static bool SafeCommentCheck(string? comment)
		{
			if (string.IsNullOrEmpty(comment))
			{
				return true;
			}

			return comment.Length <= 500;
		}

		private async Task<bool> ReviewDoesNotExist(ReviewDto review, CancellationToken cancellationToken)
		{
			Domain.Entities.Review? existingReview = await this._reviewAsyncRepository.GetUserReviewForProduct(review.UserName, review.ProductId);
			return existingReview == null || existingReview.Id == -1;
		}
		
		private async Task<bool> ProductExists(CreateReviewCommand command, CancellationToken cancellationToken)
		{
			return (await this._mediator.Send(new GetProductByIdQuery { Id = command.ReviewToCreate!.ProductId }, cancellationToken)).Product != null;
		}
	}
}