using System.Text.Json.Serialization;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents a shipping address in a PayPal order.
    /// </summary>
    public class Shipping
    {
        /// <summary>
        /// A classification for the method of purchase fulfillment (e.g shipping, in-store pickup, etc).
        /// Either type or options may be present, but not both.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        
        /// <summary>
        /// An array of shipping options that the payee or merchant offers to the payer to ship or pick up their items.
        /// </summary>
        [JsonPropertyName("options")]
        public IEnumerable<ShippingOptions> Options { get; set; } = null!;
        
        /// <summary>
        /// The name of the person to whom to ship the items. Supports only the full_name property.
        /// </summary>
        [JsonPropertyName("name")]
        public Name Name { get; set; } = null!;
        
        /// <summary>
        /// The address of the person to whom to ship the items.
        /// </summary>
        [JsonPropertyName("address")]
        public Address Address { get; set; } = null!;
    }
}