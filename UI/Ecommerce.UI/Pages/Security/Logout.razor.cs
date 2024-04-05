using Ecommerce.UI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace Ecommerce.UI.Pages.Security
{
	public partial class Logout
	{
		[Inject] public ISecurityService SecurityService { get; set; } = null!;
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;
		
		[CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; } = null!;

		protected override async Task OnInitializedAsync()
		{
			AuthenticationState authState = await this.AuthenticationState;

			await this.SecurityService.Logout(authState.User.Identity?.Name ?? string.Empty);
			
			this.NavigationManager.NavigateTo("/");
		}
	}
}