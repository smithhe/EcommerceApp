using Refit;

namespace Ecommerce.PayPal.Models
{
    public class Name
    {
        [AliasAs("given_name")]
        public string GivenName { get; set; } = null!;
            
        [AliasAs("surname")]
        public string Surname { get; set; } = null!;
    }
}