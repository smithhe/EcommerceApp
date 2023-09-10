using Ecommerce.Shared.Requests.Review;
using Ecommerce.Shared.Responses.Review;
using Refit;
using System.Threading.Tasks;

namespace Ecommerce.UI.Contracts.Refit
{
	public interface IReviewApiService
	{
		[Post("/api/review/create")]
		Task<ApiResponse<CreateReviewResponse>> CreateReview(CreateReviewApiRequest createReviewApiRequest);
	}
}