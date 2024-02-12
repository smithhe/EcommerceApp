using Ecommerce.PayPal.Models.Enums;
using Refit;

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
        [AliasAs("type")]
        public ShippingType? Type { get; set; }
        
        /// <summary>
        /// An array of shipping options that the payee or merchant offers to the payer to ship or pick up their items.
        /// </summary>
        [AliasAs("options")]
        public IEnumerable<ShippingOptions> Options { get; set; } = null!;
        
        /// <summary>
        /// The name of the person to whom to ship the items. Supports only the full_name property.
        /// </summary>
        [AliasAs("name")]
        public Name Name { get; set; } = null!;
        
        /// <summary>
        /// The address of the person to whom to ship the items.
        /// </summary>
        [AliasAs("address")]
        public Address Address { get; set; } = null!;
    }
}