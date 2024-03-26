using System.Threading.Tasks;
using Blazored.Toast.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;
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
        [Inject] private IToastService ToastService { get; set; } = null!;
        
        [Parameter] public string OrderId { get; set; } = null!;
        
        private OrderDto? Order { get; set; }

        private bool ShowError { get; set; } = false;
        private string ErrorMessage { get; set; } = string.Empty;
        
        protected override async Task OnInitializedAsync()
        {
            //Get the order by the order id
            GetOrderAfterSuccessfulCheckoutResponse getOrderAfterSuccessfulCheckoutResponse = await this.OrderService.GetOrderAfterSuccessfulCheckout(int.Parse(this.OrderId));
            
            //Failed to get the order
            if (getOrderAfterSuccessfulCheckoutResponse.Success == false)
            {
                this.ErrorMessage = getOrderAfterSuccessfulCheckoutResponse.Message ?? "Unexpected Error Occurred";
                this.ShowError = true;    
            }
            
            //Set the order
            this.Order = getOrderAfterSuccessfulCheckoutResponse.Order;
        }
        
        private double GetTotal()
        {
            double total = 0;
            
            //Loop through all items in the order and add the price * quantity to the total
            foreach (OrderItemDto orderItem in this.Order!.OrderItems!)
            {
                total += orderItem.Price * orderItem.Quantity;
            }
            
            return total;
        }
        
        private void ContinueShopping_Click()
        {
            this.NavigationManager.NavigateTo("/Categories");
        }
    }
}