using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.Entities
{
	public class EcommerceUser : IdentityUser
	{
		[Required]
		[MaxLength(255)]
		public string FirstName { get; set; } = null!;
		
		[Required]
		[MaxLength(255)]
		public string LastName { get; set; } = null!;
	}
}