using Ecommerce.Shared.Requests.Product;
using Ecommerce.Shared.Responses.Product;
using Ecommerce.UI.Contracts;
using Ecommerce.UI.Contracts.Refit;
using Newtonsoft.Json;
using Refit;
using System.Threading.Tasks;

namespace Ecommerce.UI.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductApiService _productApiService;

		public ProductService(IProductApiService productApiService)
		{
			this._productApiService = productApiService;
		}
		
		public async Task<GetAllProductsByCategoryIdResponse> GetAllProducts(int categoryId)
		{
			ApiResponse<GetAllProductsByCategoryIdResponse> response = await this._productApiService.GetAllCategories(new GetAllProductsByCategoryIdApiRequest { CategoryId = categoryId });

			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}

			if (string.IsNullOrEmpty(response.Error.Content))
			{
				return new GetAllProductsByCategoryIdResponse { Success = false, Message = "Unexpected Error Occurred" };
			}

			GetAllProductsByCategoryIdResponse? error = JsonConvert.DeserializeObject<GetAllProductsByCategoryIdResponse>(response.Error.Content);
			return error!;
		}
	}
}