using System.Text.Json.Serialization;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents a Universal Product Code (UPC).
    /// </summary>
    public class UPC
    {
        /// <summary>
        /// The Universal Product Code type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;
        
        /// <summary>
        /// The UPC product code of the item.
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; } = null!;
    }
}