using Blazored.Toast.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.CartItem;
using Ecommerce.Shared.Responses.Product;
using Ecommerce.UI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecommerce.UI.Pages
{
	public partial class Cart
	{
		[Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;
		[Inject] public IToastService ToastService { get; set; } = null!;
		[Inject] public ICartService CartService { get; set; } = null!;
		[Inject] public IProductService ProductService { get; set; } = null!;
		
		private IEnumerable<CartItemDto>? CartItems { get; set; }
		private List<ProductDto> Products { get; set; } = new List<ProductDto>();
		
		protected override async Task OnInitializedAsync()
		{
			//Pull UserId from the claims of the authenticated user if the user is logged in
			AuthenticationState authState = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
			string? userId = authState.User.Claims.FirstOrDefault(claim => string.Equals(claim.Type, ClaimTypes.NameIdentifier))?.Value;

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
				this.CartItems = Array.Empty<CartItemDto>();
			}
			else
			{
				this.CartItems = getUserCartItemsResponse.CartItems;
			}

			//Check if any items are in the cart
			IEnumerable<CartItemDto> cartItemDtos = this.CartItems.ToArray();
			if (cartItemDtos.Any() == false)
			{
				return;
			}
			
			//Get the unique product ids out of the cart
			IEnumerable<int> productIds = cartItemDtos.Select(cartItem => cartItem.ProductId).Distinct();

			foreach (int productId in productIds)
			{
				GetProductByIdResponse getProductResponse = await this.ProductService.GetProductById(productId);

				if (getProductResponse.Success)
				{
					this.Products.Add(getProductResponse.Product!);
				}
			}
		}

		private void EditCartItem(CartItemDto cartItem)
		{
			
		}

		private void RemoveCartItem(CartItemDto cartItem)
		{
			
		}

		private void PayPalCheckoutClick()
		{
			
		}

		private void StripeCheckoutClick()
		{
			
		}
	}
}