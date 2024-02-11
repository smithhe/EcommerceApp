using Refit;

namespace Ecommerce.PayPal.Models
{
    public class PayPal
    {
        [AliasAs("experience_context")]
        public ExperienceContext ExperienceContext { get; set; } = null!;
        
        [AliasAs("billing_agreement_id")]
        public string? BillingAgreementId { get; set; }
        
        [AliasAs("vault_id")]
        public string? VaultId { get; set; }
        
        [AliasAs("email_address")]
        public string? EmailAddress { get; set; }
        
        [AliasAs("name")]
        public Name? Name { get; set; }
        
        [AliasAs("phone")]
        public Phone? Phone { get; set; }
        
        [AliasAs("birth_date")]
        public string? BirthDate { get; set; }
        
        [AliasAs("tax_info")]
        public TaxInfo? TaxInfo { get; set; }
        
        [AliasAs("address")]
        public Address? Address { get; set; }
        
    }
}