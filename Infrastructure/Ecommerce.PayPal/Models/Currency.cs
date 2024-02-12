using System.ComponentModel.DataAnnotations;
using Refit;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents a currency amount in PayPal.
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// The three-character ISO-4217 currency code that identifies the currency.
        /// </summary>
        [AliasAs("currency_code")]
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = null!;

        /// <summary>
        /// The value, which might be:
        /// An integer for currencies like JPY that are not typically fractional.
        /// A decimal fraction for currencies like TND that are subdivided into thousandths.
        /// For the required number of decimal places for a currency code, see PayPal Currency Codes.
        /// </summary>
        [AliasAs("value")]
        [MaxLength(32)]
        public string Value { get; set; } = null!;
    }
}