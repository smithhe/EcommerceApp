using System;

namespace Ecommerce.Shared.Dtos
{
	public class ReviewDto
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public string UserName { get; set; } = null!;
		public int Stars { get; set; }
		public string? Comments { get; set; }
	}
}