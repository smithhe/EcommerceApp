using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Responses.Product
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to create a Product
	/// </summary>
	public class CreateProductResponse : BaseResponse
	{
		/// <summary>
		/// The newly created Product if request was successful
		/// </summary>
		public ProductDto? Product { get; set; }
	}
}