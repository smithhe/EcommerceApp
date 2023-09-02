using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Category;

namespace Ecommerce.Shared.Responses.Category
{
	/// <summary>
	/// A implementation of the <see cref="BaseResponse" /> for a <see cref="CreateCategoryApiRequest"/>
	/// </summary>
	public class CreateCategoryResponse : BaseResponse
	{
		/// <summary>
		/// The newly created Category if request was successful
		/// </summary>
		public CategoryDto? CategoryDto { get; set; }
	}
}