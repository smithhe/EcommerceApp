using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Domain.Entities
{
	public class EcommerceUser : IdentityUser
	{
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
	}
}