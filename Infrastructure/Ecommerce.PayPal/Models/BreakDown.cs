using System.Text.Json.Serialization;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents a breakdown of the total amount in a PayPal purchase unit.
    /// </summary>
    public class BreakDown
    {
        /// <summary>
        /// The subtotal for all items.
        /// Required if the request includes purchase_units[].items[].unit_amount.
        /// Must equal the sum of (items[].unit_amount * items[].quantity) for all items.
        /// item_total.value can not be a negative number.
        /// </summary>
        [JsonPropertyName("item_total")]
        public Currency? ItemTotal { get; set; }
        
        /// <summary>
        /// The shipping fee for all items within a given purchase_unit.
        /// shipping.value can not be a negative number.
        /// </summary>
        [JsonPropertyName("shipping")]
        public Currency? Shipping { get; set; }
        
        /// <summary>
        /// The handling fee for all items within a given purchase_unit.
        /// handling.value can not be a negative number.
        /// </summary>
        [JsonPropertyName("handling")]
        public Currency? Handling { get; set; }
        
        /// <summary>
        /// The total tax for all items.
        /// Required if the request includes purchase_units.items.tax. Must equal the sum of (items[].tax * items[].quantity) for all items.
        /// tax_total.value can not be a negative number.
        /// </summary>
        [JsonPropertyName("tax_total")]
        public Currency? TaxTotal { get; set; }
        
        /// <summary>
        /// The insurance fee for all items within a given purchase_unit.
        /// insurance.value can not be a negative number.
        /// </summary>
        [JsonPropertyName("insurance")]
        public Currency? Insurance { get; set; }
        
        /// <summary>
        /// The shipping discount for all items within a given purchase_unit.
        /// shipping_discount.value can not be a negative number.
        /// </summary>
        [JsonPropertyName("shipping_discount")]
        public Currency? ShippingDiscount { get; set; }
        
        /// <summary>
        /// The discount for all items within a given purchase_unit.
        /// discount.value can not be a negative number.
        /// </summary>
        [JsonPropertyName("discount")]
        public Currency? Discount { get; set; }
    }
}