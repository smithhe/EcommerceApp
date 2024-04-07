using System.ComponentModel.DataAnnotations;
using Ecommerce.Domain.Common;
// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Ecommerce.Domain.Entities
{
	public class Review : AuditableEntity
	{
		public int Id { get; set; }
		
		public int ProductId { get; set; }
		
		[MaxLength(256)]
		public string UserName { get; set; } = null!;
		
		public int Stars { get; set; }
		
		public string? Comments { get; set; }
	}
}