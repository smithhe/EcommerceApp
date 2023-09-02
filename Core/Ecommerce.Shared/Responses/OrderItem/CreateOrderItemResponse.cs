using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Responses.OrderItem
{
	/// <summary>
	/// A implementation of the <see cref="BaseResponse" /> for a requests to create an OrderItem
	/// </summary>
	public class CreateOrderItemResponse : BaseResponse
	{
		public OrderItemDto? OrderItem { get; set; }
	}
}