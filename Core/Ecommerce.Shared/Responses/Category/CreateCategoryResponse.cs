using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Responses.Category
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to create a Category
	/// </summary>
	public class CreateCategoryResponse : BaseResponse
	{
		/// <summary>
		/// The newly created Category if request was successful
		/// </summary>
		public CategoryDto? Category { get; set; }
	}
}