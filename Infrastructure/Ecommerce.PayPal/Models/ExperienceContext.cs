using Ecommerce.PayPal.Models.Enums;
using Refit;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents the experience context for a PayPal wallet payment source.
    /// </summary>
    public class ExperienceContext
    {
        /// <summary>
        /// The label that overrides the business name in the PayPal account on the PayPal site.
        /// The pattern is defined by an external party and supports Unicode.
        /// </summary>
        [AliasAs("brand_name")]
        public string? BrandName { get; set; }
        
        /// <summary>
        /// The location from which the shipping address is derived.
        /// </summary>
        [AliasAs("shipping_preference")]
        public ShippingPreference? ShippingPreference { get; set; }
        
        /// <summary>
        /// The type of landing page to show on the PayPal site for customer checkout.
        /// </summary>
        [AliasAs("landing_page")]
        public LandingPage? LandingPage { get; set; }
        
        /// <summary>
        /// Configures a Continue or Pay Now checkout flow.
        /// </summary>
        [AliasAs("user_action")]
        public UserAction? UserAction { get; set; }
        
        /// <summary>
        /// The merchant-preferred payment methods.
        /// </summary>
        [AliasAs("payment_method_preference")]
        public PaymentMethodPreference? PaymentMethodPreference { get; set; }
        
        /// <summary>
        /// The BCP 47-formatted locale of pages that the PayPal payment experience shows.
        /// PayPal supports a five-character code.
        /// For example, da-DK, he-IL, id-ID, ja-JP, no-NO, pt-BR, ru-RU, sv-SE, th-TH, zh-CN, zh-HK, or zh-TW.
        /// </summary>
        [AliasAs("locale")]
        public string? Locale { get; set; }
        
        /// <summary>
        /// The URL where the customer will be redirected upon approving a payment.
        /// </summary>
        [AliasAs("return_url")]
        public string? ReturnUrl { get; set; }
        
        /// <summary>
        /// The URL where the customer will be redirected upon cancelling the payment approval.
        /// </summary>
        [AliasAs("cancel_url")]
        public string? CancelUrl { get; set; }
    }
}