using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Review
{
	public class UpdateReviewApiRequest
	{
		public ReviewDto? ReviewToUpdate { get; set; }
	}
}