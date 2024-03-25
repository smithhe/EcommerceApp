namespace Ecommerce.Shared.Dtos
{
	public class OrderItemDto
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public string ProductName { get; set; } = null!;
		public string ProductDescription { get; set; } = null!;
		public string ProductSku { get; set; } = null!;
		public int Quantity { get; set; }
		public double Price { get; set; }
	}
}