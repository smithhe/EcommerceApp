using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Review
{
	/// <summary>
	/// A Api request to update a Review
	/// </summary>
	public class UpdateReviewApiRequest
	{
		/// <summary>
		/// The Review to update with
		/// </summary>
		public ReviewDto? ReviewToUpdate { get; set; }
	}
}