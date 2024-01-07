using System.Security.Claims;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using Ecommerce.Shared.Security;
using Ecommerce.UI.Contracts;

namespace Ecommerce.UI.Pages
{
	public partial class Profile
	{
		[CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; } = null!;
		
		[Inject] public IToastService ToastService { get; set; } = null!;
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;
		[Inject] public ISecurityService SecurityService { get; set; } = null!;

		private string? Username { get; set; }
		private string? UpdateUsername { get; set; }
		private string? Email { get; set; }
		private string? FirstName { get; set; }
		private string? LastName { get; set; }
		
		protected override async Task OnInitializedAsync()
		{
			AuthenticationState authState = await this.AuthenticationState;

			if (authState.User.Identity?.IsAuthenticated == false)
			{
				this.NavigationManager.NavigateTo("/Login");
				return;
			}
			
			this.Username = authState.User.Identity?.Name ?? string.Empty;
			this.UpdateUsername = this.Username;
			this.Email = authState.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
			this.FirstName = authState.User.FindFirst(CustomClaims._firstName)?.Value ?? string.Empty;
			this.LastName = authState.User.FindFirst(CustomClaims._lastName)?.Value ?? string.Empty;
		}
		
		private async Task UpdateProfileClick()
		{
			UpdateEcommerceUserResponse response = await this.SecurityService.UpdateUser(new UpdateEcommerceUserRequest
			{
				FirstName = this.FirstName,
				LastName = this.LastName,
				UpdateUserName = this.UpdateUsername,
				UserName = this.Username,
				Email = this.Email
			});
			
			if (response.Success)
			{
				this.ToastService.ShowSuccess("Profile Updated Successfully");
				return;
			}

			if (response.ValidationErrors?.Count > 0)
			{
				foreach (string error in response.ValidationErrors)
				{
					this.ToastService.ShowWarning(error);
				}
			}
			
			if (string.IsNullOrEmpty(response.Message) == false)
			{
				this.ToastService.ShowError(response.Message);
			}
		}
		
		private void UpdatePasswordClick()
		{
			this.ToastService.ShowInfo("Not Implemented Yet");
		}
	}
}