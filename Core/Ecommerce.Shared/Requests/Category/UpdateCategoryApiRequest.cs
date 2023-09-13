using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Category
{
	/// <summary>
	/// A Api request to update a Category
	/// </summary>
	public class UpdateCategoryApiRequest
	{
		/// <summary>
		/// The Category to update with
		/// </summary>
		public CategoryDto? CategoryToUpdate { get; set; }
	}
}