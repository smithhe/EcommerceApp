using System;

namespace Ecommerce.Shared.Requests.CartItem
{
	public class DeleteUserCartItemsApiRequest
	{
		public Guid UserId { get; set; }
	}
}