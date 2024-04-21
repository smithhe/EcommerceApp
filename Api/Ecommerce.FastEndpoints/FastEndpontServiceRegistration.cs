using Ecommerce.Application;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.FastEndpoints.Services;
using Ecommerce.Identity;
using FastEndpoints;
using FastEndpoints.Swagger;
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
			services.AddFastEndpoints()
				.SwaggerDocument(options =>
				{
					options.DocumentSettings = s =>
					{
						s.Title = "Ecommerce API";
						s.Version = "v1";
						s.Description = "API documentation for the Ecommerce application";
					};
					
					options.EndpointFilter = (endpoint) => endpoint.EndpointTags == null;
					options.AutoTagPathSegmentIndex = 0;
				});
			
			//Add the application services
			services.AddApplicationServices(configuration);

			//Add Security
			services.AddIdentityServices(configuration);
		}
	}
}