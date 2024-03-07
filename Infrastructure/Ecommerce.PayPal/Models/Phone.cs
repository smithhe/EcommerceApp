using Ecommerce.PayPal.Models.Enums;
using Refit;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents a phone number in PayPal.
    /// </summary>
    public class Phone
    {
        /// <summary>
        /// The phone type.
        /// </summary>
        [AliasAs("phone_type")]
        public string? PhoneType { get; set; }
        
        /// <summary>
        /// The phone number, in its canonical international E.164 numbering plan format.
        /// Supports only the national_number property.
        /// </summary>
        [AliasAs("phone_number")]
        public PhoneNumber PhoneNumber { get; set; } = null!;
    }
    
    /// <summary>
    /// Represents a phone number.
    /// </summary>
    public class PhoneNumber
    {
        /// <summary>
        /// The national number, in its canonical international E.164 numbering plan format.
        /// The combined length of the country calling code (CC) and the national number must not be greater than 15 digits.
        /// The national number consists of a national destination code (NDC) and subscriber number (SN).
        /// </summary>
        [AliasAs("national_number")]
        public string NationalNumber { get; set; } = null!;
    }
}