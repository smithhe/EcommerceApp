using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities
{
	public class Rating : AuditableEntity
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public int Stars { get; set; }
		public string? Comments { get; set; }
	}
}