using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Category
{
	/// <summary>
	/// A Api request to create a new Category
	/// </summary>
	public class CreateCategoryApiRequest
	{
		/// <summary>
		/// The Category to be created
		/// </summary>
		public CategoryDto? CategoryToCreate { get; set; }
	}
}