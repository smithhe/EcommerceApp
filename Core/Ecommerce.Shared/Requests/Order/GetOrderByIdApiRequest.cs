namespace Ecommerce.Shared.Requests.Order
{
	/// <summary>
	/// A Api request to get a Order via its Id
	/// </summary>
	public class GetOrderByIdApiRequest
	{
		/// <summary>
		/// The unique identifier of the Order to retrieve
		/// </summary>
		public int Id { get; set; }
	}
}