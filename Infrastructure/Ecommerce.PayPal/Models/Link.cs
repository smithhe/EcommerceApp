using Refit;

namespace Ecommerce.PayPal.Models
{
    public class Link
    {
        [AliasAs("href")]
        public string Href { get; set; }

        [AliasAs("rel")]
        public string Rel { get; set; }

        [AliasAs("method")]
        public string Method { get; set; }
    }
}