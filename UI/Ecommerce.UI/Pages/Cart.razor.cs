using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.CartItem;
using Ecommerce.Shared.Responses.Product;
using Ecommerce.UI.Contracts;
using Ecommerce.UI.Modals;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecommerce.Shared.Responses.Order;

namespace Ecommerce.UI.Pages
{
	public partial class Cart
	{
		[CascadingParameter] public IModalService Modal { get; set; } = null!;
		[Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;
		[Inject] public IToastService ToastService { get; set; } = null!;
		[Inject] public ICartService CartService { get; set; } = null!;
		[Inject] public IProductService ProductService { get; set; } = null!;
		[Inject] public IOrderService OrderService { get; set; } = null!;
		
		private List<CartItemDto>? CartItems { get; set; }
		private List<ProductDto> Products { get; set; } = new List<ProductDto>();
		private double CartTotal { get; set; }

		protected override async Task OnInitializedAsync()
		{
			//Pull UserId from the claims of the authenticated user if the user is logged in
			AuthenticationState authState = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
			string? userId = authState.User.Claims
				.FirstOrDefault(claim => string.Equals(claim.Type, ClaimTypes.NameIdentifier))?.Value;

			//Check for a value to verify user is logged in
			if (string.IsNullOrEmpty(userId))
			{
				this.NavigationManager.NavigateTo("/Login");
				return;
			}

			//Get the items in the cart
			GetUserCartItemsResponse getUserCartItemsResponse = await this.CartService.GetItemsInCart(new Guid(userId));

			//Check for success in fetching cart items
			if (getUserCartItemsResponse.Success == false)
			{
				this.ToastService.ShowError(getUserCartItemsResponse.Message!);
				return;
			}

			this.CartItems = getUserCartItemsResponse.CartItems.OrderBy(c => c.ProductId).ToList();
			
			//Check if any items are in the cart
			if (this.CartItems.Count == 0)
			{
				return;
			}

			//Get the unique product ids out of the cart
			IEnumerable<int> productIds = this.CartItems.Select(cartItem => cartItem.ProductId).Distinct();

			foreach (int productId in productIds)
			{
				GetProductByIdResponse getProductResponse = await this.ProductService.GetProductById(productId);

				if (getProductResponse.Success)
				{
					this.Products.Add(getProductResponse.Product!);
				}
			}
			
			this.CalculateCartTotal();
		}

		private void CalculateCartTotal()
		{
			this.CartTotal = 0;
			foreach (CartItemDto cartItem in this.CartItems!)
			{
				ProductDto? product = this.Products.FirstOrDefault(p => p.Id == cartItem.ProductId);
				this.CartTotal += cartItem.Quantity * product?.Price ?? 0;
			}
		}

		private void StartShoppingClick()
		{
			this.NavigationManager.NavigateTo("/Categories");
		}

		private async Task EditCartItem(CartItemDto cartItem)
		{
			ProductDto product = this.Products.First(p => p.Id == cartItem.ProductId);

			ModalParameters parameters = new ModalParameters
			{
				{ nameof(EditCartItemModal.SelectedProduct), product }, 
				{ nameof(EditCartItemModal.CurrentQuantity), cartItem.Quantity}
			};

			IModalReference formModal = this.Modal.Show<EditCartItemModal>("Update Quantity", parameters);
			ModalResult result = await formModal.Result;

			if (result.Cancelled)
			{
				return;
			}

			CartItemDto updatedCartItem = new CartItemDto
			{
				Id = cartItem.Id,
				ProductId = cartItem.ProductId,
				Quantity = (int)result.Data!,
				UserId = cartItem.UserId
			};

			UpdateCartItemResponse updateCartItemResponse = await this.CartService.UpdateItemInCart(updatedCartItem);

			if (updateCartItemResponse.Success)
			{
				this.CartItems!.Remove(cartItem);
				this.CartItems.Add(updatedCartItem);
				this.CartItems = this.CartItems.OrderBy(c => c.ProductId).ToList();
				this.CalculateCartTotal();
			}
			else if (updateCartItemResponse.ValidationErrors.Any())
			{
				this.ToastService.ShowWarning(updateCartItemResponse.Message!);
				
				foreach (string validationError in updateCartItemResponse.ValidationErrors)
				{
					this.ToastService.ShowError(validationError);
				}
			}
			else
			{
				this.ToastService.ShowError(updateCartItemResponse.Message!);
			}
		}

		private async Task RemoveCartItem(CartItemDto cartItem)
		{
			DeleteCartItemResponse response = await this.CartService.RemoveItemFromCart(cartItem);

			if (response.Success)
			{
				this.CartItems!.Remove(cartItem);
				this.CalculateCartTotal();
				return;
			}

			this.ToastService.ShowError(response.Message!);
		}

		private async Task ClearCart()
		{
			AuthenticationState authState = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();

			Claim idClaim = authState.User.Claims.First(c => string.Equals(c.Type, ClaimTypes.NameIdentifier));

			DeleteUserCartItemsResponse response = await this.CartService.ClearCart(new Guid(idClaim.Value));

			if (response.Success)
			{
				this.CartItems!.Clear();
			}
			else
			{
				this.ToastService.ShowError(response.Message!);
			}
		}
		
		private async Task PayPalCheckoutClick()
		{
			CreateOrderResponse response = await this.OrderService.CreateOrder(this.CartItems!);
			
			if (response.Success && string.IsNullOrEmpty(response.RedirectUrl) == false)
			{
				this.NavigationManager.NavigateTo(response.RedirectUrl);
			}
			else
			{
				this.ToastService.ShowError(response.Message!);
			}
		}
	}
}