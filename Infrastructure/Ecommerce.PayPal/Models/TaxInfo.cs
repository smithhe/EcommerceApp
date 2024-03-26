using System.Text.Json.Serialization;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents the tax information of the PayPal account holder.
    /// </summary>
    public class TaxInfo
    {
        /// <summary>
        /// The customer's tax ID value.
        /// </summary>
        [JsonPropertyName("tax_id")]
        public string TaxId { get; set; } = null!;
        
        /// <summary>
        /// The customer's tax ID type.
        /// </summary>
        [JsonPropertyName("tax_id_type")]
        public string TaxIdType { get; set; } = null!;
    }
}