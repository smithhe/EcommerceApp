using Refit;

namespace Ecommerce.PayPal.Models.Requests
{
    public class PayPalCreateOrderRequest
    {
        [AliasAs("intent")] 
        public string Intent { get; set; } = null!;

        [AliasAs("purchase_units")] 
        public IEnumerable<PurchaseUnit> PurchaseUnits { get; set; } = null!;
    }
}