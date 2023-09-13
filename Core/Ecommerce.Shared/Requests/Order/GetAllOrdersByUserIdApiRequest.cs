using System;

namespace Ecommerce.Shared.Requests.Order
{
	/// <summary>
	/// A Api request to get all Orders for a User
	/// </summary>
	public class GetAllOrdersByUserIdApiRequest
	{
		/// <summary>
		/// Id of the User to find all Orders for
		/// </summary>
		public Guid UserId { get; set; }
	}
}