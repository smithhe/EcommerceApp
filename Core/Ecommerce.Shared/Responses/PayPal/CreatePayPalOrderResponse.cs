
namespace Ecommerce.Shared.Responses.PayPal
{
    /// <summary>
    /// A implementation of <see cref="BaseResponse" /> for a request to create a PayPal Order
    /// </summary>
    public class CreatePayPalOrderResponse : BaseResponse
    {
        /// <summary>
        /// The url to redirect the user to for the PayPal Order
        /// </summary>
        public string? RedirectUrl { get; set; }
    }
}