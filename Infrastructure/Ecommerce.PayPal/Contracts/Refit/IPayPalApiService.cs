using Ecommerce.Shared.Requests.PayPal;
using Ecommerce.Shared.Responses.PayPal;
using Refit;

namespace Ecommerce.PayPal.Contracts.Refit
{
    public interface IPayPalApiService
    {
        [Post("/v2/checkout/orders")]
        [Headers("Content-Type: application/json")]
        Task<ApiResponse<CreatePayPalOrderResponse>> CreatePayPalOrder([Header("PayPal-Request-Id")] string payPalRequestId, [Body] CreatePayPalOrderApiRequest createPayPalOrderApiRequest);
    }
}