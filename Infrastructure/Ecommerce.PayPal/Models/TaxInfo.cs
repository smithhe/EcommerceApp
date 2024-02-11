using Refit;

namespace Ecommerce.PayPal.Models
{
    public class TaxInfo
    {
        [AliasAs("tax_id")]
        public string TaxId { get; set; } = null!;
        
        [AliasAs("tax_id_type")]
        public string TaxIdType { get; set; } = null!;
    }
}