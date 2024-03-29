using System.Text.Json.Serialization;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents a shipping option in a PayPal order.
    /// </summary>
    public class ShippingOptions
    {
        /// <summary>
        /// A unique ID that identifies a payer-selected shipping option.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;
        
        /// <summary>
        /// A description that the payer sees, which helps them choose an appropriate shipping option.
        /// For example, Free Shipping or USPS Priority Shipping.
        /// Localize this description to the payer's locale.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; } = null!;
        
        /// <summary>
        /// If the API request sets selected = true, it represents the shipping option that the payee or merchant expects
        /// to be pre-selected for the payer when they first view the shipping.options in the PayPal Checkout experience.
        /// As part of the response if a shipping.option contains selected=true, it represents the shipping option that
        /// the payer selected during the course of checkout with PayPal.
        /// Only one shipping.option can be set to selected=true.
        /// </summary>
        [JsonPropertyName("selected")]
        public bool Selected { get; set; }

        /// <summary>
        /// A classification for the method of purchase fulfillment.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;
        
        /// <summary>
        /// The shipping cost for the selected option.
        /// </summary>
        [JsonPropertyName("amount")]
        public Currency Amount { get; set; } = null!;
    }
}