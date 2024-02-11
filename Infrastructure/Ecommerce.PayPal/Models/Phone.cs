using Refit;

namespace Ecommerce.PayPal.Models
{
    public class Phone
    {
        [AliasAs("phone_type")]
        public string? PhoneType { get; set; }
        
        [AliasAs("phone_number")]
        public PhoneNumber PhoneNumber { get; set; } = null!;
    }
    
    public class PhoneNumber
    {
        [AliasAs("national_number")]
        public string NationalNumber { get; set; } = null!;
    }
}