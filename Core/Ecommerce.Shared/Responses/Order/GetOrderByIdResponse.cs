using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Order;

namespace Ecommerce.Shared.Responses.Order
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a <see cref="GetOrderByIdApiRequest"/>
	/// </summary>
	public class GetOrderByIdResponse : BaseResponse
	{
		/// <summary>
		/// The Order with the Id from the <see cref="GetOrderByIdApiRequest"/> if it exists
		/// </summary>
		public OrderDto? Order { get; set; }
	}
}