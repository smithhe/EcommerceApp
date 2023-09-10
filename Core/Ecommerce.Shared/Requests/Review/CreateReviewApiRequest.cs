using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Review
{
	public class CreateReviewApiRequest
	{
		
		public ReviewDto? ReviewToCreate { get; set; }
	}
}