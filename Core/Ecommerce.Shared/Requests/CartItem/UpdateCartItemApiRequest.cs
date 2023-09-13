using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.CartItem
{
	/// <summary>
	/// A Api request to update a CartItem
	/// </summary>
	public class UpdateCartItemApiRequest
	{
		/// <summary>
		/// The CartItem to update with
		/// </summary>
		public CartItemDto? CartItemToUpdate { get; set; }
	}
}