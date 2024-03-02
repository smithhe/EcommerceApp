using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Ecommerce.UI.Pages.CheckoutReturn
{
    public partial class CancelReturn
    {
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;

        protected override Task OnInitializedAsync()
        {
            this.NavigationManager.NavigateTo("/Cart");
            return base.OnInitializedAsync();
        }
    }
}