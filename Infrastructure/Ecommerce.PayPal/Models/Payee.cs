using System.Text.Json.Serialization;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents a merchant in PayPal.
    /// </summary>
    public class Payee
    {
        /// <summary>
        /// The email address of merchant.
        /// </summary>
        [JsonPropertyName("email_address")]
        public string? Email { get; set; }
        
        /// <summary>
        /// The encrypted PayPal account ID of the merchant.
        /// </summary>
        [JsonPropertyName("merchant_id")]
        public string? MerchantId { get; set; }
    }
}