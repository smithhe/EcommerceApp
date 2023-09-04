using Ecommerce.Domain.Common;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities
{
	public class Product : AuditableEntity
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
		public double Price { get; set; }
		public decimal AverageRating { get; set; }
		public int QuantityAvailable { get; set; }
		public string ImageUrl { get; set; } = null!;
		public Category Category { get; set; } = null!;
		public IEnumerable<Review> CustomerReviews { get; set; } = null!;
	}
}