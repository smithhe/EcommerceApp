using System;

namespace Ecommerce.Shared.Requests.CartItem
{
	/// <summary>
	/// A Api request to delete all CartItems for a User
	/// </summary>
	public class DeleteUserCartItemsApiRequest
	{
		/// <summary>
		/// The unique identifier of the User to delete all CartItems for
		/// </summary>
		public Guid UserId { get; set; }
	}
}