using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Category;

namespace Ecommerce.Shared.Responses.Category
{
	/// <summary>
	/// A implementation of the <see cref="BaseResponse" /> for a <see cref="GetCategoryByIdApiRequest"/>
	/// </summary>
	public class GetCategoryByIdResponse : BaseResponse
	{
		/// <summary>
		/// The Category with the Id from the <see cref="GetCategoryByIdApiRequest"/> if it exists
		/// </summary>
		public CategoryDto? Category { get; set; }
	}
}