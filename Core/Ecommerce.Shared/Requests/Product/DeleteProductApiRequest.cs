using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Product
{
	/// <summary>
	/// A Api request to delete a Product
	/// </summary>
	public class DeleteProductApiRequest
	{
		/// <summary>
		/// The Product to delete
		/// </summary>
		public ProductDto? ProductToDelete { get; set; }
	}
}