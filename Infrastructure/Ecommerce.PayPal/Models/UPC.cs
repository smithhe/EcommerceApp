using Ecommerce.PayPal.Models.Enums;
using Refit;

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
        [AliasAs("type")]
        public UpcType Type { get; set; }
        
        /// <summary>
        /// The UPC product code of the item.
        /// </summary>
        [AliasAs("code")]
        public string Code { get; set; } = null!;
    }
}