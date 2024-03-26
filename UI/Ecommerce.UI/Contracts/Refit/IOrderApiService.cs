using System.Threading.Tasks;
using Ecommerce.Shared.Requests.Order;
using Ecommerce.Shared.Responses.Order;
using Refit;

namespace Ecommerce.UI.Contracts.Refit
{
    public interface IOrderApiService
    {
        [Get("/api/order/user/all")]
        Task<ApiResponse<GetAllOrdersByUserIdResponse>> GetUserOrders(GetAllOrdersByUserIdApiRequest getAllOrdersByUserIdApiRequest);
        
        [Put("/api/order/update")]
        Task<ApiResponse<UpdateOrderResponse>> UpdateOrder(UpdateOrderApiRequest updateOrderApiRequest);
        
        [Get("/api/order")]
        Task<ApiResponse<GetOrderByIdResponse>> GetOrderById(GetOrderByIdApiRequest getOrderByIdApiRequest);
        
        [Post("/api/order/create")]
        Task<ApiResponse<CreateOrderResponse>> CreateOrder(CreateOrderApiRequest createOrderApiRequest);
    }
}