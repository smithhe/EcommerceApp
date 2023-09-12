using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Product;

namespace Ecommerce.Shared.Responses.Product
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to get a Product by Id
	/// </summary>
	public class GetProductByIdResponse : BaseResponse
	{
		/// <summary>
		/// The Category with the Id from the <see cref="GetProductByIdApiRequest"/> if it exists
		/// </summary>
		public ProductDto? Product { get; set; }
	}
}