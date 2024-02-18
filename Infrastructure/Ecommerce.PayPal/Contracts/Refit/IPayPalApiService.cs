using Ecommerce.PayPal.Models.Requests;
using Ecommerce.PayPal.Models.Responses;
using Refit;

namespace Ecommerce.PayPal.Contracts.Refit
{
    public interface IPayPalApiService
    {
        [Post("/v2/checkout/orders")]
        [Headers("Content-Type: application/json")]
        Task<ApiResponse<PayPalCreateOrderResponse>> CreatePayPalOrder([Header("PayPal-Request-Id")] string payPalRequestId, [Body] PayPalCreateOrderRequest createPayPalOrderApiRequest);
    }
}