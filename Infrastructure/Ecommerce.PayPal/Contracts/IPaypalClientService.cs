using Ecommerce.Shared.Requests.PayPal;
using Ecommerce.Shared.Responses.PayPal;

namespace Ecommerce.PayPal.Contracts
{
    public interface IPaypalClientService
    {
        Task<CreatePayPalOrderResponse> CreateOrder(CreatePayPalOrderRequest request);
    }
}