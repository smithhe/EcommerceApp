using Blazored.Modal;
using Blazored.Modal.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.UI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecommerce.UI.Modals
{
	public partial class AddToCartModal
	{
		[CascadingParameter] BlazoredModalInstance ModalInstance { get; set; } = null!;
		[CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; } = null!;
		[Parameter] public ProductDto SelectedProduct { get; set; } = null!;

		[Inject] public ICartService CartService { get; set; } = null!;
		
		private Guid UserId { get; set; }

		private bool ShowErrorMessage { get; set; } = false;
		private string ErrorMessage { get; set; } = string.Empty;
		
		private int Quantity { get; set; } = 1;

		protected override async Task OnInitializedAsync()
		{
			AuthenticationState authState = await this.AuthenticationState;

			Claim idClaim = authState.User.Claims.First(c => string.Equals(c.Type, ClaimTypes.NameIdentifier));
			this.UserId = new Guid(idClaim.Value);
		}

		private void SubmitForm()
		{
			if (this.Quantity <= 0)
			{
				this.ErrorMessage = "Quantity must be greater than 0";
				this.ShowErrorMessage = true;
				return;
			}

			CartItemDto cartItem = new CartItemDto
			{
				ProductId = this.SelectedProduct.Id, 
				Quantity = this.Quantity, 
				UserId = this.UserId
			};

			this.ModalInstance.CloseAsync(ModalResult.Ok(cartItem));
		}
		
		private void Cancel()
		{
			this.ModalInstance.CancelAsync();
		}
	}
}