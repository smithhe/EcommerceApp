using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Review
{
	/// <summary>
	/// A Api request to create a new Review
	/// </summary>
	public class CreateReviewApiRequest
	{
		/// <summary>
		/// The Review to create
		/// </summary>
		public ReviewDto? ReviewToCreate { get; set; }
	}
}