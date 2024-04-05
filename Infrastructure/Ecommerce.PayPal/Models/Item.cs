using System.Text.Json.Serialization;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents an item in a PayPal purchase unit.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// The item name or title.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// The item quantity. Must be a whole number.
        /// </summary>
        [JsonPropertyName("quantity")]
        public string Quantity { get; set; } = null!;
            
        /// <summary>
        /// The detailed item description.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }
            
        /// <summary>
        /// The stock keeping unit (SKU) for the item.
        /// </summary>
        [JsonPropertyName("sku")]
        public string Sku { get; set; } = null!;
            
        /// <summary>
        /// The URL to the item being purchased. Visible to buyer and used in buyer experiences.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; } = null!;

        /// <summary>
        /// The item category type.
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; } = null!;

        /// <summary>
        /// The URL of the item's image.
        /// File type and size restrictions apply.
        /// An image that violates these restrictions will not be honored.
        /// </summary>
        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }
        
        /// <summary>
        /// The item price or rate per unit.
        /// If you specify unit_amount, purchase_units[].amount.breakdown.item_total is required.
        /// Must equal unit_amount * quantity for all items.
        /// unit_amount.value can not be a negative number.
        /// </summary>
        [JsonPropertyName("unit_amount")]
        public Currency UnitAmount { get; set; } = null!;
        
        /// <summary>
        /// The item tax for each unit.
        /// If tax is specified, purchase_units[].amount.breakdown.tax_total is required.
        /// Must equal tax * quantity for all items. tax.value can not be a negative number.
        /// </summary>
        [JsonPropertyName("tax")]
        public Currency Tax { get; set; } = null!;
        
        /// <summary>
        /// The Universal Product Code of the item.
        /// </summary>
        [JsonPropertyName("upc")]
        public Upc? Upc { get; set; }
    }
}