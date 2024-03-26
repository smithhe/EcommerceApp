using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;

namespace Ecommerce.UI.Contracts
{
    public interface IOrderService
    {
        Task<GetAllOrdersByUserIdResponse> GetUserOrders(Guid userId);
        Task<UpdateOrderResponse> UpdateOrder(OrderDto orderDto);
        Task<GetOrderByIdResponse> GetOrderById(int orderId);
        Task<GetOrderAfterSuccessfulCheckoutResponse> GetOrderAfterSuccessfulCheckout(int orderId);
        Task<CreateOrderResponse> CreateOrder(IEnumerable<CartItemDto> cartItems);
    }
}