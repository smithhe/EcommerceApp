using System;
using System.Threading.Tasks;
using Ecommerce.Shared.Responses.Order;

namespace Ecommerce.UI.Contracts
{
    public interface IOrderService
    {
        Task<GetAllOrdersByUserIdResponse> GetUserOrders(Guid userId);
    }
}