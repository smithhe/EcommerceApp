using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Responses.Review
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to create a Review
	/// </summary>
	public class CreateReviewResponse : BaseResponse
	{
		/// <summary>
		/// The newly created Category if request was successful
		/// </summary>
		public ReviewDto? Review { get; set; }
	}
}