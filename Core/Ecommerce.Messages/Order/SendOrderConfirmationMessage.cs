using System.Collections.Generic;
using Ecommerce.Shared.Dtos;

namespace Ecommerce.Messages.Order
{
    /// <summary>
    /// Message to send an order confirmation
    /// </summary>
    public class SendOrderConfirmationMessage
    {
        /// <summary>
        /// The email address to send the order confirmation to
        /// </summary>
        public string SendTo { get; init; } = null!;
        
        /// <summary>
        /// Name of the customer
        /// </summary>
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// Total amount of the order
        /// </summary>
        public double Total { get; set; }

        /// <summary>
        /// Order number of the order
        /// </summary>
        public string OrderNumber { get; set; } = null!;

        /// <summary>
        /// Order items of the order
        /// </summary>
        public IEnumerable<OrderItemDto> OrderItems { get; set; } = null!;
    }
}