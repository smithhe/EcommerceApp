using Ecommerce.Shared.Requests.Product;
using Ecommerce.Shared.Responses.Product;
using Refit;
using System.Threading.Tasks;

namespace Ecommerce.UI.Contracts.Refit
{
	public interface IProductApiService
	{
		[Get("/api/product/all")]
		Task<ApiResponse<GetAllProductsByCategoryIdResponse>> GetAllProductsByCategory(GetAllProductsByCategoryIdApiRequest getAllProductsRequest);
		
		[Get("/api/product/get")]
		Task<ApiResponse<GetProductByIdResponse>> GetProductById(GetProductByIdApiRequest getProductByIdApiRequest);
	}
}