using Blazored.Toast.Services;
using Ecommerce.UI.Contracts;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.UI.Security;

namespace Ecommerce.UI.Pages.Security
{
	public partial class Login
	{
		[Parameter] public string? NewUser { get; set; }
		
		[Inject] public IToastService ToastService { get; set; } = null!;
		[Inject] public ISecurityService SecurityService { get; set; } = null!;
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;

		public AuthenticationRequest AuthenticationRequestModel { get; set; } = new AuthenticationRequest();
		private bool IsMessageVisible { get; set; }
		private bool IsProcessing { get; set; } = false;
		
		private bool ShowError { get; set; }
		private string ErrorMessage { get; set; } = string.Empty;

		protected override void OnInitialized()
		{
			this.IsMessageVisible = Convert.ToBoolean(this.NewUser);
		}

		private async Task LoginButton_Click()
		{
			this.IsProcessing = true;
			LoginResponse response = await this.SecurityService.Login(this.AuthenticationRequestModel);
			this.IsProcessing = false;
			
			if (response.IsSuccessful == false)
			{
				this.ErrorMessage = response.Message!;
				this.ShowError = true;
				return;
			}
			
			this.NavigationManager.NavigateTo("/");
		}
	}
}