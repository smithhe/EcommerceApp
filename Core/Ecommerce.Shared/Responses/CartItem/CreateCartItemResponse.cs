using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.CartItem;

namespace Ecommerce.Shared.Responses.CartItem
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a <see cref="CreateCartItemApiRequest"/>
	/// </summary>
	public class CreateCartItemResponse : BaseResponse
	{
		/// <summary>
		/// The newly created CartItem if request was successful
		/// </summary>
		public CartItemDto? CartItem { get; set; }
	}
}