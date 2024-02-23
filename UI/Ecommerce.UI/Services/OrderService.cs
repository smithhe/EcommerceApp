using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Order;
using Ecommerce.Shared.Responses.Order;
using Ecommerce.UI.Contracts;
using Ecommerce.UI.Contracts.Refit;
using Newtonsoft.Json;
using Refit;

namespace Ecommerce.UI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderApiService _orderApiService;

        public OrderService(IOrderApiService orderApiService)
        {
            this._orderApiService = orderApiService;
        }

        public async Task<GetAllOrdersByUserIdResponse> GetUserOrders(Guid userId)
        {
            ApiResponse<GetAllOrdersByUserIdResponse> response = await this._orderApiService.GetUserOrders(new GetAllOrdersByUserIdApiRequest
            {
                UserId = userId
            });
			
            if (response.IsSuccessStatusCode)
            {
                return response.Content;
            }
			
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new GetAllOrdersByUserIdResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<GetAllOrdersByUserIdResponse>(response.Error.Content)!;
        }

        public async Task<UpdateOrderResponse> UpdateOrder(OrderDto orderDto)
        {
            ApiResponse<UpdateOrderResponse> response = await this._orderApiService.UpdateOrder(new UpdateOrderApiRequest
            {
                OrderToUpdate = orderDto
            });
            
            if (response.IsSuccessStatusCode)
            {
                return response.Content;
            }
			
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new UpdateOrderResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<UpdateOrderResponse>(response.Error.Content)!;
        }
        
        public async Task<GetOrderByIdResponse> GetOrderById(int orderId)
        {
            ApiResponse<GetOrderByIdResponse> response = await this._orderApiService.GetOrderById(new GetOrderByIdApiRequest
            {
                Id = orderId
            });
            
            if (response.IsSuccessStatusCode)
            {
                return response.Content;
            }
            
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new GetOrderByIdResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<GetOrderByIdResponse>(response.Error.Content)!;
        }
        
        public async Task<CreateOrderResponse> CreateOrder(IEnumerable<CartItemDto> cartItems)
        {
            ApiResponse<CreateOrderResponse> response = await this._orderApiService.CreateOrder(new CreateOrderApiRequest
            {
                CartItems = cartItems
            });
            
            if (response.IsSuccessStatusCode)
            {
                return response.Content;
            }
            
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new CreateOrderResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<CreateOrderResponse>(response.Error.Content)!;
        }
    }
}