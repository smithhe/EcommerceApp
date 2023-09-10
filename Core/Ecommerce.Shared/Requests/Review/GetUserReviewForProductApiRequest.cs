namespace Ecommerce.Shared.Requests.Review
{
	public class GetUserReviewForProductApiRequest
	{
		public string? UserName { get; set; }
		
		public int ProductId { get; set; }
	}
}