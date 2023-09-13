using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Product
{
	/// <summary>
	/// A Api request to update a Product
	/// </summary>
	public class UpdateProductApiRequest
	{
		/// <summary>
		/// The Product to update with
		/// </summary>
		public ProductDto? ProductToUpdate { get; set; }
	}
}