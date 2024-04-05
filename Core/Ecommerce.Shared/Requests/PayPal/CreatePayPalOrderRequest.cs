using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.PayPal
{
    /// <summary>
    /// A request to model the information needed to create a PayPal Order
    /// </summary>
    public class CreatePayPalOrderRequest
    {
        /// <summary>
        /// The Ecommerce order to create a PayPal Order for
        /// </summary>
        public OrderDto? Order { get; set; }

        /// <summary>
        /// The return key to map the PayPal Order to the Ecommerce Order
        /// </summary>
        public string ReturnKey { get; set; } = null!;
    }
}