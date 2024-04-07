using System.Collections.Generic;

namespace Ecommerce.Shared.Dtos
{
	public class ProductDto
	{
		public int Id { get; set; }
		public int CategoryId { get; set; }
		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
		public double Price { get; set; }
		public decimal AverageRating { get; set; }
		public int QuantityAvailable { get; set; }
		public string ImageUrl { get; set; } = null!;
		public IEnumerable<ReviewDto> CustomerReviews { get; set; } = null!;
	}
}