using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Review;

namespace Ecommerce.Shared.Responses.Review
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a <see cref="CreateReviewApiRequest"/>
	/// </summary>
	public class CreateReviewResponse : BaseResponse
	{
		/// <summary>
		/// The newly created Category if request was successful
		/// </summary>
		public ReviewDto? Review { get; set; }
	}
}