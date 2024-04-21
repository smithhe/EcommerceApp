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
			ApiResponse<GetAllProductsByCategoryIdResponse> response = await this._productApiService.GetAllProductsByCategory(new GetAllProductsByCategoryIdApiRequest { CategoryId = categoryId });

			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}
			
			return string.IsNullOrEmpty(response.Error.Content) ? 
				new GetAllProductsByCategoryIdResponse { Success = false, Message = "Unexpected Error Occurred" } 
				: JsonConvert.DeserializeObject<GetAllProductsByCategoryIdResponse>(response.Error.Content)!;
		}

		public async Task<GetProductByIdResponse> GetProductById(int productId)
		{
			ApiResponse<GetProductByIdResponse> response = await this._productApiService.GetProductById(new GetProductByIdApiRequest { ProductId = productId });

			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}

			return string.IsNullOrEmpty(response.Error.Content) ? 
				new GetProductByIdResponse { Success = false, Message = "Unexpected Error Occurred" } 
				: JsonConvert.DeserializeObject<GetProductByIdResponse>(response.Error.Content)!;
		}
	}
}