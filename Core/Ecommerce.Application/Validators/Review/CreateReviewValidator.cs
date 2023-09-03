using Ecommerce.Application.Features.Review.Commands.CreateReview;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Validators.Review
{
	public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
	{
		private readonly IReviewAsyncRepository _reviewAsyncRepository;
		private readonly IProductAsyncRepository _productAsyncRepository;

		public CreateReviewValidator(IReviewAsyncRepository reviewAsyncRepository, IProductAsyncRepository productAsyncRepository)
		{
			this._reviewAsyncRepository = reviewAsyncRepository;
			this._productAsyncRepository = productAsyncRepository;

			RuleFor(c => c.ReviewToCreate!)
				.MustAsync(ReviewExists).WithMessage("Review already exists for this product");
			
			RuleFor(c => c)
				.MustAsync(ProductExists).WithMessage("Product must exist");
			
			//TODO: Check that user exists

			RuleFor(c => c.ReviewToCreate!.Stars)
				.GreaterThanOrEqualTo(0).WithMessage("Number of stars must be greater than or equal to 0");

			RuleFor(c => c.ReviewToCreate!.Comments)
				.Must(SafeCommentCheck).WithMessage("Comment must not exceed 500 characters");
		}

		private bool SafeCommentCheck(string? comment)
		{
			if (string.IsNullOrEmpty(comment))
			{
				return true;
			}

			return comment.Length <= 500;
		}

		private async Task<bool> ReviewExists(ReviewDto review, CancellationToken cancellationToken)
		{
			return (await this._reviewAsyncRepository.GetUserReviewForProduct(review.UserId, review.ProductId)) == null;
		}
		
		private async Task<bool> ProductExists(CreateReviewCommand command, CancellationToken cancellationToken)
		{
			return (await this._productAsyncRepository.GetByIdAsync(command.ReviewToCreate!.ProductId)) == null;
		}
	}
}