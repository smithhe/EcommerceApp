using Ecommerce.Shared.Responses.Category;
using System.Threading.Tasks;

namespace Ecommerce.UI.Contracts
{
	public interface ICategoryService
	{
		Task<GetAllCategoriesResponse> GetAllCategories();
	}
}