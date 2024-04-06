using Ecommerce.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Domain.Entities
{
	public class CartItem : AuditableEntity
	{
		public int Id { get; set; }
		
		public int ProductId { get; set; }
		
		[Column (TypeName = "varchar(255)")]
		public Guid UserId { get; set; }
		
		public int Quantity { get; set; }
	}
}