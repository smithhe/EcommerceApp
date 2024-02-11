using Ecommerce.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Ecommerce.PayPal;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Application
{
	public static class ApplicationServiceRegistration
	{
		public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddAutoMapper(Assembly.GetExecutingAssembly());
			services.AddMediatR(cfg =>
			{
				cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
			});
			
			services.AddPersistenceServices();
			services.AddPayPalServices(configuration);
		}
	}
}