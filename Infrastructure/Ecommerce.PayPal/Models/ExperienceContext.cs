using System.Text.Json.Serialization;

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
        [JsonPropertyName("brand_name")]
        public string? BrandName { get; set; }
        
        /// <summary>
        /// The location from which the shipping address is derived.
        /// </summary>
        [JsonPropertyName("shipping_preference")]
        public string? ShippingPreference { get; set; }
        
        /// <summary>
        /// The type of landing page to show on the PayPal site for customer checkout.
        /// </summary>
        [JsonPropertyName("landing_page")]
        public string? LandingPage { get; set; }
        
        /// <summary>
        /// Configures a Continue or Pay Now checkout flow.
        /// </summary>
        [JsonPropertyName("user_action")]
        public string? UserAction { get; set; }
        
        /// <summary>
        /// The merchant-preferred payment methods.
        /// </summary>
        [JsonPropertyName("payment_method_preference")]
        public string? PaymentMethodPreference { get; set; }
        
        /// <summary>
        /// The BCP 47-formatted locale of pages that the PayPal payment experience shows.
        /// PayPal supports a five-character code.
        /// For example, da-DK, he-IL, id-ID, ja-JP, no-NO, pt-BR, ru-RU, sv-SE, th-TH, zh-CN, zh-HK, or zh-TW.
        /// </summary>
        [JsonPropertyName("locale")]
        public string? Locale { get; set; }
        
        /// <summary>
        /// The URL where the customer will be redirected upon approving a payment.
        /// </summary>
        [JsonPropertyName("return_url")]
        public string? ReturnUrl { get; set; }
        
        /// <summary>
        /// The URL where the customer will be redirected upon cancelling the payment approval.
        /// </summary>
        [JsonPropertyName("cancel_url")]
        public string? CancelUrl { get; set; }
    }
}