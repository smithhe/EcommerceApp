using Refit;

namespace Ecommerce.PayPal.Models
{
    public class Item
    {
        [AliasAs("name")]
        public string Name { get; set; } = null!;
            
        [AliasAs("quantity")]
        public string Quantity { get; set; } = null!;
            
        [AliasAs("description")]
        public string? Description { get; set; }
            
        [AliasAs("sku")]
        public string Sku { get; set; } = null!;
            
        [AliasAs("category")]
        public string Category { get; set; } = null!;
            
        [AliasAs("unit_amount")]
        public Money UnitAmount { get; set; } = null!;
            
        [AliasAs("tax")]
        public Money Tax { get; set; } = null!;
    }
}