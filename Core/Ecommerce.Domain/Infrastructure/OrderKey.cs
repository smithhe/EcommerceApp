using System;

namespace Ecommerce.Domain.Infrastructure
{
    public class OrderKey
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderToken { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}