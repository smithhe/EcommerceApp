using Refit;

namespace Ecommerce.PayPal.Models
{
    public class ExperienceContext
    {
        [AliasAs("brand_name")]
        public string BrandName { get; set; } = null!;
        
        [AliasAs("shipping_preference")]
        public string ShippingPreference { get; set; } = null!;
        
        [AliasAs("landing_page")]
        public string LandingPage { get; set; } = null!;
        
        [AliasAs("user_action")]
        public string UserAction { get; set; } = null!;
        
        [AliasAs("payment_method_preference")]
        public string PaymentMethodPreference { get; set; } = null!;
        
        [AliasAs("locale")]
        public string Locale { get; set; } = null!;
        
        [AliasAs("return_url")]
        public string ReturnUrl { get; set; } = null!;
        
        [AliasAs("cancel_url")]
        public string CancelUrl { get; set; } = null!;
    }
}