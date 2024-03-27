using System.Collections.Generic;
using Ecommerce.Mail.Contracts;
using Ecommerce.Shared.Dtos;

namespace Ecommerce.Mail.Models.TemplateModels
{
    /// <summary>
    /// Model for the order receipt email template
    /// </summary>
    public class OrderReceiptModel : ITemplateModel
    {
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