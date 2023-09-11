using Ecommerce.Shared.Dtos;
using System;
using System.Collections.Generic;

namespace Ecommerce.Shared.Responses.CartItem
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to get all CartItems for a User
	/// </summary>
	public class GetUserCartItemsResponse : BaseResponse
	{
		/// <summary>
		/// The collection of CartItems if any exist for the User
		/// </summary>
		public IEnumerable<CartItemDto> Products { get; set; } = Array.Empty<CartItemDto>();
	}
}