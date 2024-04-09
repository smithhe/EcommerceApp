using System;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.Infrastructure
{
    public class OrderKey
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        
        [MaxLength(50)]
        public string OrderToken { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}