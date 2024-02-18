using Ecommerce.PayPal.Models.Enums;
using Refit;

namespace Ecommerce.PayPal.Models.Responses
{
    /// <summary>
    /// Represents the response from the PayPal API when creating an order.
    /// </summary>
    public class PayPalCreateOrderResponse
    {
        /// <summary>
        /// The date and time when the transaction occurred, in Internet date and time format.
        /// </summary>
        [AliasAs("create_time")]
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// The date and time when the transaction was last updated, in Internet date and time format.
        /// </summary>
        [AliasAs("update_time")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// The ID of the order.
        /// </summary>
        [AliasAs("id")]
        public string Id { get; set; } = null!;
        
        /// <summary>
        /// The instruction to process an order.
        /// </summary>
        [AliasAs("processing_instruction")]
        public ProcessingInstruction? ProcessingInstruction { get; set; }

        /// <summary>
        /// The order status.
        /// </summary>
        [AliasAs("status")]
        public OrderStatus? Status { get; set; }

        /// <summary>
        /// The intent to either capture payment immediately or authorize a payment for an order after order creation.
        /// </summary>
        [AliasAs("intent")]
        public Intent Intent { get; set; }

        /// <summary>
        /// An array of purchase units.
        /// Each purchase unit establishes a contract between a customer and merchant.
        /// Each purchase unit represents either a full or partial order that the customer intends to purchase from the merchant.
        /// </summary>
        [AliasAs("purchase_units")]
        public List<PurchaseUnit> PurchaseUnits { get; set; }
        
        /// <summary>
        /// An array of request-related HATEOAS links.
        /// To complete payer approval, use the approve link to redirect the payer.
        /// The API caller has 3 hours (default setting, this which can be changed by your account manager to 24/48/72 hours to accommodate your use case) from the time the order is created,
        /// to redirect your payer. Once redirected, the API caller has 3 hours for the payer to approve the order and either authorize or capture the order.
        /// </summary>
        [AliasAs("links")]
        public List<Link> Links { get; set; }
    }
}