using System;

namespace Ecommerce.Shared.Requests.CartItem
{
	/// <summary>
	/// A Api request to get all CartItems for a User
	/// </summary>
	public class GetUserCartItemsApiRequest
	{
		/// <summary>
		/// Id of the User to find all CartItems for
		/// </summary>
		public Guid UserId { get; set; }
	}
}