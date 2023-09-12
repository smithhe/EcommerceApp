using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Responses.CartItem
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to create a new CartItem
	/// </summary>
	public class CreateCartItemResponse : BaseResponse
	{
		/// <summary>
		/// The newly created CartItem if request was successful
		/// </summary>
		public CartItemDto? CartItem { get; set; }
	}
}