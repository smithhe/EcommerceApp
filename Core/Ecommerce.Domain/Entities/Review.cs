using Ecommerce.Domain.Common;
using System;

namespace Ecommerce.Domain.Entities
{
	public class Review : AuditableEntity
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public string UserName { get; set; } = null!;
		public int Stars { get; set; }
		public string? Comments { get; set; }
	}
}