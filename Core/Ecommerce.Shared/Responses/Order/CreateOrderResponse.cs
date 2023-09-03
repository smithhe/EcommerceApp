using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Order;

namespace Ecommerce.Shared.Responses.Order
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a <see cref="CreateOrderApiRequest"/>
	/// </summary>
	public class CreateOrderResponse : BaseResponse
	{
		/// <summary>
		/// The newly created Order if request was successful
		/// </summary>
		public OrderDto? Order { get; set; }
	}
}