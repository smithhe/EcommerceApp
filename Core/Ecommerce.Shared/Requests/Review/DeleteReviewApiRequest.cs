using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Review
{
	public class DeleteReviewApiRequest
	{
		public ReviewDto? ReviewToDelete { get; set; }
	}
}