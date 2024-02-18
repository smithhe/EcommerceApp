using System.Collections.Generic;
using Ecommerce.Shared.Dtos;

namespace Ecommerce.Shared.Requests.PayPal
{
    /// <summary>
    /// A request to model the information needed to create a PayPal Order
    /// </summary>
    public class CreatePayPalOrderRequest
    {
        /// <summary>
        /// The Ecommerce order to create a PayPal Order for
        /// </summary>
        public OrderDto? Order { get; set; }
        
        /// <summary>
        /// The product information for the products in the order
        /// </summary>
        public IEnumerable<ProductDto>? OrderProducts { get; set; }
    }
}