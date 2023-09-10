using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Review;

namespace Ecommerce.Shared.Responses.Review
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a <see cref="GetUserReviewForProductApiRequest"/>
	/// </summary>
	public class GetUserReviewForProductResponse : BaseResponse
	{
		/// <summary>
		/// The User's <see cref="ReviewDto"/> if it exists
		/// </summary>
		public ReviewDto? UserReview { get; set; }
	}
}