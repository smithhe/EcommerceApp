using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Responses.OrderItem
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a requests to create an OrderItem
	/// </summary>
	public class CreateOrderItemResponse : BaseResponse
	{
		/// <summary>
		/// The OrderItem created if request was successful
		/// </summary>
		public OrderItemDto? OrderItem { get; set; }
	}
}