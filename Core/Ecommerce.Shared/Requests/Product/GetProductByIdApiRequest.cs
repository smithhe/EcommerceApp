namespace Ecommerce.Shared.Requests.Product
{
	/// <summary>
	/// A Api request to get a Product via its Id
	/// </summary>
	public class GetProductByIdApiRequest
	{
		/// <summary>
		/// The unique identifier of the Product to retrieve
		/// </summary>
		public int ProductId { get; set; }
	}
}