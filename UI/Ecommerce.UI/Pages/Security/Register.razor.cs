using Blazored.Toast.Services;
using Ecommerce.UI.Contracts;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;

namespace Ecommerce.UI.Pages.Security
{
	public partial class Register
	{
		[Inject] public ISecurityService SecurityService { get; set; } = null!;
		[Inject] public IToastService ToastService { get; set; } = null!;
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;

		private CreateUserRequest CreateUserRequestModel { get; set; } = new CreateUserRequest();
		private List<string>? RegistrationErrors { get; set; }
		private bool IsProcessing { get; set; } = false;

		private async Task CreateUserButton_Click()
		{
			this.IsProcessing = true;
			CreateUserResponse response = await this.SecurityService.RegisterUser(this.CreateUserRequestModel);
			this.IsProcessing = false;
			
			if (response.Success)
			{
				this.NavigationManager.NavigateTo("/Login/true");
				return;
			}

			this.RegistrationErrors = new List<string>();
			this.RegistrationErrors.AddRange(response.Errors!);
			
			this.ToastService.ShowError("Error creating your account, please try again later");
		}
	}
}