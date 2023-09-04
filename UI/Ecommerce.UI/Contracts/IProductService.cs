using Ecommerce.Shared.Responses.Product;
using System.Threading.Tasks;

namespace Ecommerce.UI.Contracts
{
	public interface IProductService
	{
		Task<GetAllProductsByCategoryIdResponse> GetAllProducts(int categoryId);
		Task<GetProductByIdResponse> GetProductById(int productId);
	}
}