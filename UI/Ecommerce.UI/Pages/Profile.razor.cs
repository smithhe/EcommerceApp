using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace Ecommerce.UI.Pages
{
	public partial class Profile
	{
		[CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; } = null!;
		
		[Inject] public IToastService ToastService { get; set; } = null!;
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;

		private string? Username { get; set; }
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
			
		}
		
		
		private void UpdateProfileClick()
		{
			this.ToastService.ShowInfo("Not Implemented Yet");
		}
		
		private void UpdatePasswordClick()
		{
			this.ToastService.ShowInfo("Not Implemented Yet");
		}
	}
}