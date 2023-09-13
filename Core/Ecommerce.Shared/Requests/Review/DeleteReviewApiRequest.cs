using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Review
{
	/// <summary>
	/// A Api request to delete a Review
	/// </summary>
	public class DeleteReviewApiRequest
	{
		/// <summary>
		/// The Review to delete
		/// </summary>
		public ReviewDto? ReviewToDelete { get; set; }
	}
}