using Ecommerce.Application;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.FastEndpoints.Services;
using Ecommerce.Identity;
using FastEndpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.FastEndpoints
{
	public static class FastEndpointServiceRegistration
	{
		public static void AddFastEndpointServices(this IServiceCollection services, IConfiguration configuration)
		{
			//Add the services for Fast Endpoints
			services.AddScoped<ITokenService, TokenService>();
			
			//https://fast-endpoints.com/
			services.AddFastEndpoints();
			
			//TODO: Add swagger
			
			//Add the application services
			services.AddApplicationServices(configuration);

			//Add Security
			services.AddIdentityServices(configuration);
		}
	}
}