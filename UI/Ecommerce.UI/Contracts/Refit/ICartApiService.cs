using Ecommerce.Shared.Requests.CartItem;
using Ecommerce.Shared.Responses.CartItem;
using Refit;
using System.Threading.Tasks;

namespace Ecommerce.UI.Contracts.Refit
{
	public interface ICartApiService
	{
		[Post("/api/cartitem/create")]
		Task<ApiResponse<CreateCartItemResponse>> CreateCartItem(CreateCartItemApiRequest createCartItemApiRequest);
		
		[Delete("/api/cartitem/delete")]
		Task<ApiResponse<DeleteCartItemResponse>> DeleteCartItem([Body] DeleteCartItemApiRequest deleteCartItemApiRequest);
		
		[Delete("/api/cartitem/delete")]
		Task<ApiResponse<DeleteUserCartItemsResponse>> DeleteAllCartItems([Body] DeleteUserCartItemsApiRequest deleteUserCartItemsApiRequest);
		
		[Put("/api/cartitem/update")]
		Task<ApiResponse<UpdateCartItemResponse>> UpdateCartItem(UpdateCartItemApiRequest updateCartItemApiRequest);
		
		[Get("/api/cartitem/all")]
		Task<ApiResponse<GetUserCartItemsResponse>> GetUserCartItems(GetUserCartItemsApiRequest getUserCartItemsApiRequest);
	}
}