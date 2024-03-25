using System;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;
using Ecommerce.UI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Ecommerce.UI.Pages
{
    public partial class OrderDetail
    {
        [Parameter] public string OrderId { get; set; } = null!;
        
        [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; } = null!;
        [Inject] public IToastService ToastService { get; set; } = null!;
        
        [Inject] public IOrderService OrderService { get; set; } = null!;
        [Inject] public IProductService ProductService { get; set; } = null!;
        
        private OrderDto? Order { get; set; }
        private string? OrderStatus { get; set; }

        protected override async Task OnInitializedAsync()
        {
            GetOrderByIdResponse getOrderByIdResponse = await this.OrderService.GetOrderById(int.Parse(this.OrderId));
            
            //Failed to get the order
            if (getOrderByIdResponse.Success == false)
            {
                this.ToastService.ShowError(getOrderByIdResponse.Message!);    
                return;
            }
            
            //Successfully got the order
            this.Order = getOrderByIdResponse.Order;
            this.OrderStatus = Enum.GetName(this.Order!.Status);
        }
        
        
    }
}