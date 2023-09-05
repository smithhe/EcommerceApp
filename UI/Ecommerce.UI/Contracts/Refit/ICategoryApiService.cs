using Ecommerce.Shared.Requests.Category;
using Ecommerce.Shared.Responses.Category;
using Refit;
using System.Threading.Tasks;

namespace Ecommerce.UI.Contracts.Refit
{
	public interface ICategoryApiService
	{
		[Get("/api/category/all")]
		Task<ApiResponse<GetAllCategoriesResponse>> GetAllCategories(GetAllCategoriesApiRequest getAllCategoriesApiRequest);
	}
}