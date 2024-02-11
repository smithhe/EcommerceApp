using Refit;

namespace Ecommerce.PayPal.Models
{
    public class ShippingOptions
    {
        [AliasAs("id")]
        public string Id { get; set; } = null!;
            
        [AliasAs("label")]
        public string Label { get; set; } = null!;
            
        [AliasAs("selected")]
        public bool Selected { get; set; }
            
        [AliasAs("type")]
        public string Type { get; set; } = null!;
            
        [AliasAs("amount")]
        public Money Amount { get; set; } = null!;
    }
}