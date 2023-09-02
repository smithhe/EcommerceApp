using Ecommerce.Persistence.Contracts;
using Ecommerce.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Persistence
{
	public static class PersistenceServiceRegistration
	{
		public static void AddPersistenceServices(this IServiceCollection services)
		{
			services.AddScoped<ICategoryAsyncRepository, CategoryAsyncRepository>();
			services.AddScoped<IOrderAsyncRepository, OrderAsyncRepository>();
			services.AddScoped<IOrderItemAsyncRepository, OrderItemAsyncRepository>();
			services.AddScoped<IProductAsyncRepository, ProductAsyncRepository>();
			services.AddScoped<IReviewAsyncRepository, ReviewAsyncRepository>();
		}
	}
}