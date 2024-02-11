using System;

namespace Ecommerce.Shared.Responses.PayPal
{
    /// <summary>
    /// A implementation of <see cref="BaseResponse" /> for a request to create a PayPal Order
    /// </summary>
    public class CreatePayPalOrderResponse : BaseResponse
    {
        /// <summary>
        /// The unique identifier for the PayPal request to help ensure Idempotency
        /// </summary>
        public Guid PayPalRequestId { get; set; }
    }
}