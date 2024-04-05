using System;
using System.Threading.Tasks;
using Ecommerce.Shared.Security.Responses;
using Ecommerce.UI.Contracts;
using Ecommerce.UI.Extensions;
using Microsoft.AspNetCore.Components;

namespace Ecommerce.UI.Pages.Security
{
    public partial class ConfirmEmail
    {

        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        [Inject] public ISecurityService SecurityService { get; set; } = null!;
        
        private bool Loading { get; set; } = true;
        private string Message { get; set; } = string.Empty;
        
        protected override async Task OnInitializedAsync()
        {
            string? userId, emailToken;
            Uri uri = this.NavigationManager.ToAbsoluteUri(this.NavigationManager.Uri);
            
            this.NavigationManager.TryGetQueryString("userId", out userId);
            this.NavigationManager.TryGetQueryString("emailToken", out emailToken);
            
            ConfirmEmailResponse response = await this.SecurityService.ConfirmUserEmail(userId, emailToken);
            
            if (response.Success)
            {
                this.Message = "Email Confirmed Successfully!";
            }
            else
            {
                this.Message = response.Message ?? "An Error Occurred While Confirming Your Email";
            }
            
            this.Loading = false;
        }
    }
}