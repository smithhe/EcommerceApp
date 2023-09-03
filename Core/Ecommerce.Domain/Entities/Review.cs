using Ecommerce.Domain.Common;
using System;

namespace Ecommerce.Domain.Entities
{
	public class Review : AuditableEntity
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public Guid UserId { get; set; }
		public int Stars { get; set; }
		public string? Comments { get; set; }
	}
}