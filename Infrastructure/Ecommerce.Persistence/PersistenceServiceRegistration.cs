using Ecommerce.Persistence.Contracts;
using Ecommerce.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Ecommerce.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Persistence
{
	/// <summary>
	/// Extension class of IServiceCollection to register services
	/// </summary>
	public static class PersistenceServiceRegistration
	{
		/// <summary>
		/// Extension method to register services for the Persistence project
		/// </summary>
		public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
		{
			//Register the DbContext
			services.AddDbContext<EcommercePersistenceDbContext>(options =>
			{
				options.UseMySQL(configuration.GetConnectionString("datastorage")!);
			});
			
			//Register the repositories
			services.AddScoped<ICategoryAsyncRepository, CategoryAsyncRepository>();
			services.AddScoped<IOrderAsyncRepository, OrderAsyncRepository>();
			services.AddScoped<IOrderItemAsyncRepository, OrderItemAsyncRepository>();
			services.AddScoped<IProductAsyncRepository, ProductAsyncRepository>();
			services.AddScoped<IReviewAsyncRepository, ReviewAsyncRepository>();
			services.AddScoped<ICartItemRepository, CartItemRepository>();
			services.AddScoped<IOrderKeyRepository, OrderKeyRepository>();
			
			//Register services
			services.AddScoped<IStorageService, ProductImageStorageService>();
		}
		
		
	}
}