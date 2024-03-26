using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ecommerce.PayPal.Models
{
    public class PurchaseAmount
    {
        /// <summary>
        /// The three-character ISO-4217 currency code that identifies the currency.
        /// </summary>
        [JsonPropertyName("currency_code")]
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = null!;

        /// <summary>
        /// The value, which might be:
        /// An integer for currencies like JPY that are not typically fractional.
        /// A decimal fraction for currencies like TND that are subdivided into thousandths.
        /// For the required number of decimal places for a currency code, see PayPal Currency Codes.
        /// </summary>
        [JsonPropertyName("value")]
        [MaxLength(32)]
        public string Value { get; set; } = null!;
        
        /// <summary>
        /// The breakdown of the amount.
        /// Breakdown provides details such as total item amount, total tax amount, shipping, handling, insurance, and discounts, if any.
        /// </summary>
        [JsonPropertyName("breakdown")]
        public BreakDown? BreakDown { get; set; }
    }
}