using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Product
{
	/// <summary>
	/// A Api request to create a new Product
	/// </summary>
	public class CreateProductApiRequest
	{
		/// <summary>
		/// The Product to be created
		/// </summary>
		public ProductDto? ProductToCreate { get; set; }
	}
}