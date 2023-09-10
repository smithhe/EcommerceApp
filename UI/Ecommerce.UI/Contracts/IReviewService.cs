using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Review;
using System.Threading.Tasks;

namespace Ecommerce.UI.Contracts
{
	public interface IReviewService
	{
		Task<CreateReviewResponse> SubmitReview(ReviewDto review);

		Task<GetUserReviewForProductResponse> GetUserReview(string userName, int productId);
	}
}