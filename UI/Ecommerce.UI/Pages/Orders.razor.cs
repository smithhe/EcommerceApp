using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Enums;
using Ecommerce.Shared.Responses.Order;
using Ecommerce.UI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Ecommerce.UI.Pages
{
    public partial class Orders
    {
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        [Inject] public IToastService ToastService { get; set; } = null!;
        
        [Inject] public IOrderService OrderService { get; set; } = null!;
        
        private IEnumerable<OrderDto>? UserOrders { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //Pull UserId from the claims of the authenticated user if the user is logged in
            AuthenticationState authState = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
            string? userId = authState.User.Claims
                .FirstOrDefault(claim => string.Equals(claim.Type, ClaimTypes.NameIdentifier))?.Value;

            //Check for a value to verify user is logged in
            if (string.IsNullOrEmpty(userId))
            {
                this.NavigationManager.NavigateTo("/Login");
                return;
            }
            
            //Load in the user's orders
            await this.GetUserOrders(userId);
        }

        private async Task CancelOrder(OrderDto order)
        {
            //Pull UserId from the claims of the authenticated user if the user is logged in
            AuthenticationState authState = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
            string? userId = authState.User.Claims
                .FirstOrDefault(claim => string.Equals(claim.Type, ClaimTypes.NameIdentifier))?.Value;
            
            //Verify the order is in a status that can be cancelled
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
            {
                this.ToastService.ShowWarning("Order cannot be cancelled at this time.");
                return;
            }
            
            //Update the order status to cancelled
            order.Status = OrderStatus.Cancelled;
            
            //Update the order
            UpdateOrderResponse updateOrderResponse = await this.OrderService.UpdateOrder(order);

            //Check for success in updating the order
            if (updateOrderResponse.Success)
            {
                //Reload the user's orders
                await this.GetUserOrders(userId!);
            }
        }
        
        private async Task ShowOrderDetails(OrderDto order)
        {
            
        }

        private async Task GetUserOrders(string userId)
        {
            //Get the user's orders
            GetAllOrdersByUserIdResponse getUserOrdersResponse = await this.OrderService.GetUserOrders(new Guid(userId));
            
            //Check for success in fetching orders
            this.UserOrders = getUserOrdersResponse.Success
                ? getUserOrdersResponse.Orders
                : new List<OrderDto>();
        }
    }
}