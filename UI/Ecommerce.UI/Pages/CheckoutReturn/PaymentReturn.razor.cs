using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecommerce.UI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Ecommerce.UI.Pages.CheckoutReturn
{
    public partial class PaymentReturn
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
        
        [Inject] private IOrderService OrderService { get; set; } = null!;
        
        [Parameter] public string OrderId { get; set; } = null!;
        
        protected override async Task OnInitializedAsync()
        {
            
        }
        
        private void ContinueShopping_Click()
        {
            this.NavigationManager.NavigateTo("/Categories");
        }
    }
}