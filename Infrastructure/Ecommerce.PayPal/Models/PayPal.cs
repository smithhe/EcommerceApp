using System.Text.Json.Serialization;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents the PayPal wallet payment source.
    /// </summary>
    public class PayPal
    {
        /// <summary>
        /// Customizes the payer experience during the approval process for payment with PayPal.
        /// </summary>
        [JsonPropertyName("experience_context")]
        public ExperienceContext? ExperienceContext { get; set; }
        
        /// <summary>
        /// The PayPal billing agreement ID.
        /// References an approved recurring payment for goods or services.
        /// </summary>
        [JsonPropertyName("billing_agreement_id")]
        public string? BillingAgreementId { get; set; }
        
        /// <summary>
        /// The PayPal-generated ID for the payment_source stored within the Vault.
        /// </summary>
        [JsonPropertyName("vault_id")]
        public string? VaultId { get; set; }
        
        /// <summary>
        /// The email address of the PayPal account holder.
        /// </summary>
        [JsonPropertyName("email_address")]
        public string? EmailAddress { get; set; }
        
        /// <summary>
        /// The name of the PayPal account holder.
        /// Supports only the given_name and surname properties.
        /// </summary>
        [JsonPropertyName("name")]
        public Name? Name { get; set; }
        
        /// <summary>
        /// The phone number of the customer.
        /// Available only when you enable the Contact Telephone Number option in the Profile & Settings for the merchant's PayPal account.
        /// </summary>
        [JsonPropertyName("phone")]
        public Phone? Phone { get; set; }
        
        /// <summary>
        /// The birth date of the PayPal account holder in YYYY-MM-DD format.
        /// </summary>
        [JsonPropertyName("birth_date")]
        public string? BirthDate { get; set; }
        
        /// <summary>
        /// The tax information of the PayPal account holder.
        /// Required only for Brazilian PayPal account holder's.
        /// Both tax_id and tax_id_type are required.
        /// </summary>
        [JsonPropertyName("tax_info")]
        public TaxInfo? TaxInfo { get; set; }
        
        /// <summary>
        /// The address of the PayPal account holder.
        /// Supports only the address_line_1, address_line_2, admin_area_1, admin_area_2, postal_code, and country_code properties.
        /// Also referred to as the billing address of the customer.
        /// </summary>
        [JsonPropertyName("address")]
        public Address? Address { get; set; }
        
    }
}