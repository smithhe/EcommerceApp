using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.CartItem
{
	/// <summary>
	/// A Api request to create a new CartItem
	/// </summary>
	public class CreateCartItemApiRequest
	{
		/// <summary>
		/// The CartItem to be created
		/// </summary>
		public CartItemDto? CartItemToCreate { get; set; }
	}
}