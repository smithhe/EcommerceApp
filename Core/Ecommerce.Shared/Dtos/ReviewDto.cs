namespace Ecommerce.Shared.Dtos
{
	public class ReviewDto
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public int Stars { get; set; }
		public string? Comments { get; set; }
	}
}