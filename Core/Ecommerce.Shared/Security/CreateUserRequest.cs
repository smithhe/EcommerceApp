using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Security
{
	public class CreateUserRequest
	{
		[Required]
		public string? UserName { get; set; }
		[Required]
		public string? FirstName { get; set; }
		[Required]
		public string? LastName { get; set; }
		[Required]
		public string? EmailAddress { get; set; }
		[Required]
		[MaxLength(50)]
		public string? Password { get; set; }
	}
}