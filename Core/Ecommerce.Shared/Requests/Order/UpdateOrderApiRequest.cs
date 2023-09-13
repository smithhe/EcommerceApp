using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Order
{
	/// <summary>
	/// A Api request to update a Order
	/// </summary>
	public class UpdateOrderApiRequest
	{
		/// <summary>
		/// The Order to update with
		/// </summary>
		public OrderDto? OrderToUpdate { get; set; }
	}
}