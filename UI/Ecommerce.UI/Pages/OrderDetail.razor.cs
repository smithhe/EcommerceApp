using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;
using Ecommerce.Shared.Responses.Product;
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
        private List<ProductDto> OrderProducts { get; set; } = new List<ProductDto>();
        
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
            
            //Pull the list of product ids in the order
            IEnumerable<int> productIds = this.Order.OrderItems!.Select(orderItem => orderItem.ProductId).Distinct();
            
            //Fetch the information for each product in the order
            foreach (int productId in productIds)
            {
                GetProductByIdResponse getProductByIdResponse = await this.ProductService.GetProductById(productId);
                
                //Failed to get the product
                if (getProductByIdResponse.Success == false)
                {
                    this.ToastService.ShowError("Failed to load order products.");
                    return;
                }
                
                //Successfully got the product
                ProductDto product = getProductByIdResponse.Product!;
                
                //Add the product to the list of products in the order
                this.OrderProducts.Add(product);
            }
        }
        
        
    }
}