using Ecommerce.PayPal.Models.Enums;
using Refit;

namespace Ecommerce.PayPal.Models.Requests
{
    /// <summary>
    /// Represents a request to create a PayPal order.
    /// </summary>
    public class PayPalCreateOrderRequest
    {
        /// <summary>
        /// The intent to either capture payment immediately or authorize a payment for an order after order creation.
        /// </summary>
        [AliasAs("intent")] 
        public Intent Intent { get; set; }

        /// <summary>
        /// An array of purchase units. Each purchase unit establishes a contract between a payer and the payee.
        /// Each purchase unit represents either a full or partial order that the payer intends to purchase from the payee.
        /// </summary>
        [AliasAs("purchase_units")] 
        public IEnumerable<PurchaseUnit> PurchaseUnits { get; set; } = null!;
        
        /// <summary>
        /// The payment source definition.
        /// </summary>
        [AliasAs("payment_source")]
        public PaymentSource PaymentSource { get; set; } = null!;
    }
}