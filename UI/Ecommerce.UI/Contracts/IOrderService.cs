using System;
using System.Threading.Tasks;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;

namespace Ecommerce.UI.Contracts
{
    public interface IOrderService
    {
        Task<GetAllOrdersByUserIdResponse> GetUserOrders(Guid userId);
        Task<UpdateOrderResponse> UpdateOrder(OrderDto orderDto);
    }
}