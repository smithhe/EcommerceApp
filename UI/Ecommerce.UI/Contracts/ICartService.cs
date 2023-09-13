using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.CartItem;
using System;
using System.Threading.Tasks;

namespace Ecommerce.UI.Contracts
{
	public interface ICartService
	{
		Task<CreateCartItemResponse> AddItemToCart(CartItemDto cartItem);
		
		Task<DeleteCartItemResponse> RemoveItemFromCart(CartItemDto cartItem);

		Task<DeleteUserCartItemsResponse> ClearCart(Guid userId);
		
		Task<UpdateCartItemResponse> UpdateItemInCart(CartItemDto cartItem);
		
		Task<GetUserCartItemsResponse> GetItemsInCart(Guid userId);
	}
}