using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Responses.Review
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to get a User's Review for a Product
	/// </summary>
	public class GetUserReviewForProductResponse : BaseResponse
	{
		/// <summary>
		/// The User's <see cref="ReviewDto"/> if it exists
		/// </summary>
		public ReviewDto? UserReview { get; set; }
	}
}