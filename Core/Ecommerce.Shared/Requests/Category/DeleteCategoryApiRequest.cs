using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.Category
{
	/// <summary>
	/// A Api request to delete a Category
	/// </summary>
	public class DeleteCategoryApiRequest
	{
		/// <summary>
		/// The Category to delete
		/// </summary>
		public CategoryDto? CategoryToDelete { get; set; }
	}
}