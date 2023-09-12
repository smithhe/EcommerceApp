using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Category;

namespace Ecommerce.Shared.Responses.Category
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to get a Category by Id
	/// </summary>
	public class GetCategoryByIdResponse : BaseResponse
	{
		/// <summary>
		/// The Category with the Id from the <see cref="GetCategoryByIdApiRequest"/> if it exists
		/// </summary>
		public CategoryDto? Category { get; set; }
	}
}