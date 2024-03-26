using System.Text.Json.Serialization;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents the payment source for a PayPal order.
    /// </summary>
    public class PaymentSource
    {
        /// <summary>
        /// The PayPal payment source.
        /// </summary>
        [JsonPropertyName("paypal")]
        public PayPal? PayPal { get; set; }
    }
}