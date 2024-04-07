using Ecommerce.Domain.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Ecommerce.Domain.Entities
{
	public class Product : AuditableEntity
	{
		public int Id { get; set; }
		
		public int CategoryId { get; set; }
		
		[MaxLength(255)]
		public string Name { get; set; } = null!;
		
		public string Description { get; set; } = null!;
		
		public double Price { get; set; }
		
		public decimal AverageRating { get; set; }
		
		public int QuantityAvailable { get; set; }
		

		public string ImageUrl { get; set; } = null!;
		
		public IEnumerable<Review> CustomerReviews { get; set; } = null!;
	}
}