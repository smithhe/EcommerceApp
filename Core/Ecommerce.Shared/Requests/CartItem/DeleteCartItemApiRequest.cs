using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.CartItem
{
	/// <summary>
	/// A Api request to delete a CartItem
	/// </summary>
	public class DeleteCartItemApiRequest
	{
		/// <summary>
		/// The CartItem to delete
		/// </summary>
		public CartItemDto? CartItemToDelete { get; set; }
	}
}