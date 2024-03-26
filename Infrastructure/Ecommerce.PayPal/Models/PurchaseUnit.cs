using Ecommerce.PayPal.Models.Requests;
using Refit;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents a purchase unit in a PayPal order.
    /// </summary>
    public class PurchaseUnit
    {
        /// <summary>
        /// The API caller-provided external ID for the purchase unit.
        /// Required for multiple purchase units when you must update the order through PATCH.
        /// If you omit this value and the order contains only one purchase unit, PayPal sets this value to default.
        /// </summary>
        [AliasAs("reference_id")]
        public string ReferenceId { get; set; } = null!;

        /// <summary>
        /// The purchase description.
        /// The maximum length of the character is dependent on the type of characters used.
        /// The character length is specified assuming a US ASCII character.
        /// Depending on type of character; (e.g. accented character, Japanese characters) the number of characters that that can be specified as input might not equal the permissible max length.
        /// </summary>
        [AliasAs("description")]
        public string Description { get; set; } = null!;
            
        /// <summary>
        /// The API caller-provided external ID.
        /// Used to reconcile client transactions with PayPal transactions.
        /// Appears in transaction and settlement reports but is not visible to the payer.
        /// </summary>
        [AliasAs("custom_id")]
        public string? CustomId { get; set; }
            
        /// <summary>
        /// The API caller-provided external invoice number for this order.
        /// Appears in both the payer's transaction history and the emails that the payer receives.
        /// </summary>
        [AliasAs("invoice_id")]
        public string? InvoiceId { get; set; }

        /// <summary>
        /// The soft descriptor is the dynamic text used to construct the statement descriptor that appears on a payer's card statement.
        /// </summary>
        [AliasAs("soft_descriptor")]
        public string SoftDescriptor { get; set; } = null!;
            
        /// <summary>
        /// An array of items that the customer purchases from the merchant.
        /// </summary>
        [AliasAs("items")]
        public IEnumerable<Item> Items { get; set; } = null!;
            
        /// <summary>
        /// The total order amount with an optional breakdown that provides details, such as the total item amount, total tax amount, shipping, handling, insurance, and discounts, if any.
        /// If you specify amount.breakdown, the amount equals item_total plus tax_total plus shipping plus handling plus insurance minus shipping_discount minus discount.
        /// The amount must be a positive number. The amount.value field supports up to 15 digits preceding the decimal.
        /// For a list of supported currencies, decimal precision, and maximum charge amount, see the PayPal REST APIs Currency Codes.
        /// </summary>
        [AliasAs("amount")]
        public PurchaseAmount Amount { get; set; } = null!;
            
        /// <summary>
        /// The merchant who receives payment for this transaction.
        /// </summary>
        [AliasAs("payee")]
        public Payee? Payee { get; set; }
        
        /// <summary>
        /// The name and address of the person to whom to ship the items.
        /// </summary>
        [AliasAs("shipping")]
        public Shipping? Shipping { get; set; }
    }
}