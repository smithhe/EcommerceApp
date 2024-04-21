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
using System.Threading.Tasks;

namespace Ecommerce.UI.Pages
{
	public partial class Products
	{
		[CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; } = null!;
		[CascadingParameter] public IModalService Modal { get; set; } = null!;
		[Parameter] public string CategoryId { get; set; } = null!;

		[Inject] public NavigationManager NavigationManager { get; set; } = null!;
		[Inject] public IProductService ProductService { get; set; } = null!;
		[Inject] public IToastService ToastService { get; set; } = null!;
		[Inject] public ICartService CartService { get; set; } = null!;
		
		private IEnumerable<ProductDto>? ProductList { get; set; }
		
		protected override async Task OnInitializedAsync()
		{
			GetAllProductsByCategoryIdResponse response = await this.ProductService.GetAllProducts(Convert.ToInt32(this.CategoryId));
			
			if (response.Success)
			{
				this.ProductList = response.Products;
			}
			else
			{
				this.ToastService.ShowError(response.Message!);
			}
		}

		private async Task AddToCartClick(int productId)
		{
			AuthenticationState authState = await this.AuthenticationState;

			if (authState.User.Identity?.IsAuthenticated == false)
			{
				this.NavigationManager.NavigateTo("/Login");
				return;
			}
			
			ProductDto product = this.ProductList!.First(p => p.Id == productId);

			ModalParameters parameters = new ModalParameters { { nameof(AddToCartModal.SelectedProduct), product } };
			
			IModalReference formModal = this.Modal.Show<AddToCartModal>("Add Item To Cart", parameters);
			ModalResult result = await formModal.Result;

			if (result.Cancelled)
			{
				return;
			}

			CartItemDto newCartItem = (CartItemDto)result.Data!;

			CreateCartItemResponse response = await this.CartService.AddItemToCart(newCartItem);

			if (response.Success)
			{
				this.ToastService.ShowSuccess(response.Message!);
				return;
			}
			
			
			if (response.ValidationErrors.Any())
			{
				foreach (string validationError in response.ValidationErrors)
				{
					this.ToastService.ShowError(validationError);
				}
			}
			else
			{
				this.ToastService.ShowError(response.Message!);
			}
		}
	}
}