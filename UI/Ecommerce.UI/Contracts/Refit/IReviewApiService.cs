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
		
		[Delete("/api/review/delete")]
		Task<ApiResponse<DeleteReviewResponse>> DeleteReview(DeleteReviewApiRequest deleteReviewApiRequest);
		
		[Put("/api/review/update")]
		Task<ApiResponse<UpdateReviewResponse>> UpdateReview(UpdateReviewApiRequest updateReviewApiRequest);
		
		[Get("/api/review/user")]
		Task<ApiResponse<GetUserReviewForProductResponse>> GetUserReview(GetUserReviewForProductApiRequest getUserReviewForProductApiRequest);
	}
}