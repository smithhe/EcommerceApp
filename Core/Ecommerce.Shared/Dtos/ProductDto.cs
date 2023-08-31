using System.Collections.Generic;

namespace Ecommerce.Shared.Dtos
{
	public class ProductDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
		public double Price { get; set; }
		public double AverageRating { get; set; }
		public int QuantityAvailable { get; set; }
		public CategoryDto Category { get; set; } = null!;
		public IEnumerable<RatingDto> CustomerRatings { get; set; } = null!;
	}
}