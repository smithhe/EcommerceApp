using Refit;

namespace Ecommerce.PayPal.Models
{
    public class Address
    {
        [AliasAs("address_line_1")]
        public string AddressLine1 { get; set; } = null!;
            
        [AliasAs("address_line_2")]
        public string? AddressLine2 { get; set; }
            
        [AliasAs("admin_area_2")]
        public string AdminArea2 { get; set; } = null!;
            
        [AliasAs("admin_area_1")]
        public string AdminArea1 { get; set; } = null!;
            
        [AliasAs("postal_code")]
        public string PostalCode { get; set; } = null!;
            
        [AliasAs("country_code")]
        public string CountryCode { get; set; } = null!;
    }
}