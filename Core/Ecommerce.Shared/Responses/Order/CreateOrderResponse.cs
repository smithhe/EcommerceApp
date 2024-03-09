using System;
using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Responses.Order
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to create a Order
	/// </summary>
	public class CreateOrderResponse : BaseResponse
	{
		/// <summary>
		/// The newly created Order if request was successful
		/// </summary>
		public OrderDto? Order { get; set; }
        
		/// <summary>
		/// The url to redirect the user to for payment of the order
		/// </summary>
		public string? RedirectUrl { get; set; }
	}
}