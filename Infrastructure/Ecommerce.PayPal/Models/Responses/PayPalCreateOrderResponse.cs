using Refit;

namespace Ecommerce.PayPal.Models.Responses
{
    public class PayPalCreateOrderResponse
    {
        [AliasAs("create_time")]
        public DateTime CreateTime { get; set; }
        
        [AliasAs("update_time")]
        public DateTime UpdateTime { get; set; }
        
        [AliasAs("id")]
        public string Id { get; set; }
        
        [AliasAs("processing_instruction")]
        public string ProcessingInstruction { get; set; }

        [AliasAs("status")]
        public string Status { get; set; }

        [AliasAs("intent")]
        public string Intent { get; set; }

        [AliasAs("purchase_units")]
        public List<PurchaseUnit> PurchaseUnits { get; set; }
        
        [AliasAs("links")]
        public List<Link> Links { get; set; }
    }
}