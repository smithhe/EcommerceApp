namespace Ecommerce.Shared.Requests.Review
{
	/// <summary>
	/// A Api request to get a User's review of a Product
	/// </summary>
	public class GetUserReviewForProductApiRequest
	{
		/// <summary>
		/// The UserName of the User who made the Review
		/// </summary>
		public string? UserName { get; set; }
		
		/// <summary>
		/// The unique identifier of the Product the Review is for
		/// </summary>
		public int ProductId { get; set; }
	}
}