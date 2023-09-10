using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Review;
using Ecommerce.Shared.Responses.Review;
using Ecommerce.UI.Contracts;
using Ecommerce.UI.Contracts.Refit;
using Newtonsoft.Json;
using Refit;
using System.Threading.Tasks;

namespace Ecommerce.UI.Services
{
	public class ReviewService : IReviewService
	{
		private readonly IReviewApiService _reviewApiService;

		public ReviewService(IReviewApiService reviewApiService)
		{
			this._reviewApiService = reviewApiService;
		}
		
		public async Task<CreateReviewResponse> SubmitReview(ReviewDto review)
		{
			ApiResponse<CreateReviewResponse> response = await this._reviewApiService.CreateReview(new CreateReviewApiRequest { ReviewToCreate = review});

			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}
			
			return string.IsNullOrEmpty(response.Error.Content) ? 
				new CreateReviewResponse { Success = false, Message = "Unexpected Error Occurred" } 
				: JsonConvert.DeserializeObject<CreateReviewResponse>(response.Error.Content)!;
		}

		public async Task<GetUserReviewForProductResponse> GetUserReview(string userName, int productId)
		{
			ApiResponse<GetUserReviewForProductResponse> response = await this._reviewApiService.GetUserReview(new GetUserReviewForProductApiRequest { ProductId = productId, UserName = userName});
			
			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}
			
			return string.IsNullOrEmpty(response.Error.Content) ? 
				new GetUserReviewForProductResponse { Success = false, Message = "Unexpected Error Occurred" } 
				: JsonConvert.DeserializeObject<GetUserReviewForProductResponse>(response.Error.Content)!;
		}
	}
}