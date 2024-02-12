using Refit;

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
        [AliasAs("tax_id")]
        public string TaxId { get; set; } = null!;
        
        /// <summary>
        /// The customer's tax ID type.
        /// </summary>
        [AliasAs("tax_id_type")]
        public string TaxIdType { get; set; } = null!;
    }
}