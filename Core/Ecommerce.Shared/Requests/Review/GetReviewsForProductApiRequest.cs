namespace Ecommerce.Shared.Requests.Review
{
	/// <summary>
	/// A Api request to get all reviews for a Product
	/// </summary>
	public class GetReviewsForProductApiRequest
	{
		/// <summary>
		/// Id of the Product to find all Reviews for
		/// </summary>
		public int ProductId { get; set; }
	}
}