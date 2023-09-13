using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Order
{
	/// <summary>
	/// A Api request to create a new Order
	/// </summary>
	public class CreateOrderApiRequest
	{
		/// <summary>
		/// The Order to be created
		/// </summary>
		public OrderDto? OrderToCreate { get; set; }
	}
}