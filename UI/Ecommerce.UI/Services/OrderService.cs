using System;
using System.Threading.Tasks;
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
    }
}