using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Product;

namespace Ecommerce.Shared.Responses.Product
{
	/// <summary>
	/// A implementation of the <see cref="BaseResponse" /> for a <see cref="CreateProductApiRequest"/>
	/// </summary>
	public class CreateProductResponse : BaseResponse
	{
		/// <summary>
		/// The newly created Product if request was successful
		/// </summary>
		public ProductDto? Product { get; set; }
	}
}