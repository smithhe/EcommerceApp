using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.CartItem;
using Ecommerce.Shared.Responses.CartItem;
using Ecommerce.UI.Contracts;
using Ecommerce.UI.Contracts.Refit;
using Newtonsoft.Json;
using Refit;
using System;
using System.Threading.Tasks;

namespace Ecommerce.UI.Services
{
	public class CartService : ICartService
	{
		private readonly ICartApiService _cartApiService;

		public CartService(ICartApiService cartApiService)
		{
			this._cartApiService = cartApiService;
		}
		
		public async Task<CreateCartItemResponse> AddItemToCart(CartItemDto cartItem)
		{
			ApiResponse<CreateCartItemResponse> response = await this._cartApiService.CreateCartItem(new CreateCartItemApiRequest { CartItemToCreate = cartItem });
			
			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}
			
			return string.IsNullOrEmpty(response.Error.Content) ? 
				new CreateCartItemResponse { Success = false, Message = "Unexpected Error Occurred" } 
				: JsonConvert.DeserializeObject<CreateCartItemResponse>(response.Error.Content)!;
		}

		public async Task<DeleteCartItemResponse> RemoveItemFromCart(CartItemDto cartItem)
		{
			ApiResponse<DeleteCartItemResponse> response = await this._cartApiService.DeleteCartItem(new DeleteCartItemApiRequest { CartItemToDelete = cartItem });
			
			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}
			
			return string.IsNullOrEmpty(response.Error.Content) ? 
				new DeleteCartItemResponse { Success = false, Message = "Unexpected Error Occurred" } 
				: JsonConvert.DeserializeObject<DeleteCartItemResponse>(response.Error.Content)!;
		}

		public async Task<DeleteUserCartItemsResponse> ClearCart(Guid userId)
		{
			ApiResponse<DeleteUserCartItemsResponse> response = await this._cartApiService.DeleteAllCartItems(new DeleteUserCartItemsApiRequest { UserId = userId });
			
			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}
			
			return string.IsNullOrEmpty(response.Error.Content) ? 
				new DeleteUserCartItemsResponse { Success = false, Message = "Unexpected Error Occurred" } 
				: JsonConvert.DeserializeObject<DeleteUserCartItemsResponse>(response.Error.Content)!;
		}

		public async Task<UpdateCartItemResponse> UpdateItemInCart(CartItemDto cartItem)
		{
			ApiResponse<UpdateCartItemResponse> response = await this._cartApiService.UpdateCartItem(new UpdateCartItemApiRequest { CartItemToUpdate = cartItem });
			
			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}
			
			return string.IsNullOrEmpty(response.Error.Content) ? 
				new UpdateCartItemResponse { Success = false, Message = "Unexpected Error Occurred" } 
				: JsonConvert.DeserializeObject<UpdateCartItemResponse>(response.Error.Content)!;
		}

		public async Task<GetUserCartItemsResponse> GetItemsInCart(Guid userId)
		{
			ApiResponse<GetUserCartItemsResponse> response = await this._cartApiService.GetUserCartItems(new GetUserCartItemsApiRequest { UserId = userId });
			
			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}
			
			return string.IsNullOrEmpty(response.Error.Content) ? 
				new GetUserCartItemsResponse { Success = false, Message = "Unexpected Error Occurred" } 
				: JsonConvert.DeserializeObject<GetUserCartItemsResponse>(response.Error.Content)!;
		}
	}
}