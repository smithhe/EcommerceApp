using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity
{
	public class EcommerceIdentityDbContext : IdentityDbContext<EcommerceUser>
	{
		public EcommerceIdentityDbContext(DbContextOptions<EcommerceIdentityDbContext> options) : base(options) 
		{
		}
	}
}