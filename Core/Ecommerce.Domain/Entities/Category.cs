using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities
{
	public class Category : AuditableEntity
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public string Summary { get; set; } = null!;
	}
}