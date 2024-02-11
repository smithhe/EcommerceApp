using Refit;

namespace Ecommerce.PayPal.Models
{
    public class Payee
    {
        [AliasAs("email_address")]
        public string Email { get; set; }
            
        [AliasAs("merchant_id")]
        public string MerchantId { get; set; }
    }
}