using Dapper;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Persistence.Helpers;
using Ecommerce.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;

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
		public static void AddPersistenceServices(this IServiceCollection services)
		{
			services.AddScoped<ICategoryAsyncRepository, CategoryAsyncRepository>();
			services.AddScoped<IOrderAsyncRepository, OrderAsyncRepository>();
			services.AddScoped<IOrderItemAsyncRepository, OrderItemAsyncRepository>();
			services.AddScoped<IProductAsyncRepository, ProductAsyncRepository>();
			services.AddScoped<IReviewAsyncRepository, ReviewAsyncRepository>();
			services.AddScoped<ICartItemRepository, CartItemRepository>();
			services.AddScoped<IOrderKeyRepository, OrderKeyRepository>();
			
			//Add handler for Guid
			SqlMapper.AddTypeHandler(new MySqlGuidTypeHandler());
			SqlMapper.RemoveTypeMap(typeof(Guid));
			SqlMapper.RemoveTypeMap(typeof(Guid?));
		}
	}
}