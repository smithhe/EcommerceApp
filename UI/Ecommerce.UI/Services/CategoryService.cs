using Ecommerce.Shared.Requests.Category;
using Ecommerce.Shared.Responses.Category;
using Ecommerce.UI.Contracts;
using Ecommerce.UI.Contracts.Refit;
using Newtonsoft.Json;
using Refit;
using System.Threading.Tasks;

namespace Ecommerce.UI.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly ICategoryApiService _categoryApiService;

		public CategoryService(ICategoryApiService categoryApiService)
		{
			this._categoryApiService = categoryApiService;
		}
		
		public async Task<GetAllCategoriesResponse> GetAllCategories()
		{
			ApiResponse<GetAllCategoriesResponse> response = await this._categoryApiService.GetAllCategories(new GetAllCategoriesApiRequest());

			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}

			if (string.IsNullOrEmpty(response.Error.Content))
			{
				return new GetAllCategoriesResponse { Success = false, Message = "Unexpected Error Occurred" };
			}

			GetAllCategoriesResponse? error = JsonConvert.DeserializeObject<GetAllCategoriesResponse>(response.Error.Content);
			return error!;
		}
	}
}