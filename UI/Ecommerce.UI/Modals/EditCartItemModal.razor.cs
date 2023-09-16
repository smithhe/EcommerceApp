using Blazored.Modal;
using Blazored.Modal.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.UI.Contracts;
using Microsoft.AspNetCore.Components;

namespace Ecommerce.UI.Modals
{
	public partial class EditCartItemModal
	{
		[CascadingParameter] BlazoredModalInstance ModalInstance { get; set; } = null!;
		[Parameter] public ProductDto SelectedProduct { get; set; } = null!;
		[Parameter] public int CurrentQuantity { get; set; }

		[Inject] public ICartService CartService { get; set; } = null!;
		
		private bool ShowErrorMessage { get; set; } = false;
		private string ErrorMessage { get; set; } = string.Empty;
		
		private int Quantity { get; set; } = 1;

		protected override void OnInitialized()
		{
			this.Quantity = this.CurrentQuantity;
		}

		private void SubmitForm()
		{
			if (this.Quantity <= 0)
			{
				this.ErrorMessage = "Quantity must be greater than 0";
				this.ShowErrorMessage = true;
				return;
			}

			this.ModalInstance.CloseAsync(ModalResult.Ok(this.Quantity));
		}
		
		private void Cancel()
		{
			this.ModalInstance.CancelAsync();
		}
	}
}