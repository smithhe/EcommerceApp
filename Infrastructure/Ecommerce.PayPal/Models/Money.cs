using Refit;

namespace Ecommerce.PayPal.Models
{
    public class Money
    {
        [AliasAs("currency_code")]
        public string CurrencyCode { get; set; }
            
        [AliasAs("value")]
        public string Value { get; set; }
    }
}