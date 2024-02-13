namespace Ecommerce.Shared.Dtos
{
	public class OrderItemDto
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public double Price { get; set; }
	}
}