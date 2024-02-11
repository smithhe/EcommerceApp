using Refit;

namespace Ecommerce.PayPal.Models
{
    public class Shipping
    {
        [AliasAs("type")]
        public string Type { get; set; } = null!;
            
        [AliasAs("options")]
        public IEnumerable<ShippingOptions> Options { get; set; } = null!;
            
        [AliasAs("name")]
        public Name Name { get; set; } = null!;
            
        [AliasAs("address")]
        public Address Address { get; set; } = null!;
    }
}