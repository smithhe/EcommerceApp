using Ecommerce.PayPal.Models.Requests;
using Refit;

namespace Ecommerce.PayPal.Models
{
    public class PurchaseUnit
    {
        [AliasAs("reference_id")]
        public string ReferenceId { get; set; } = null!;
            
        [AliasAs("description")]
        public string? Description { get; set; }
            
        [AliasAs("custom_id")]
        public string? CustomId { get; set; }
            
        [AliasAs("invoice_id")]
        public string? InvoiceId { get; set; }
            
        [AliasAs("soft_descriptor")]
        public string? SoftDescriptor { get; set; }
            
        [AliasAs("items")]
        public IEnumerable<Item> Items { get; set; } = null!;
            
        [AliasAs("amount")]
        public Money Amount { get; set; } = null!;
            
        [AliasAs("payee")]
        public Payee? Payee { get; set; }
            
        [AliasAs("shipping")]
        public Shipping? Shipping { get; set; }
    }
}