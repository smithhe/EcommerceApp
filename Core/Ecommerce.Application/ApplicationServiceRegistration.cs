using System;
using Ecommerce.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Ecommerce.PayPal;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Application
{
	public static class ApplicationServiceRegistration
	{
		public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddAutoMapper(Assembly.GetExecutingAssembly());
			
			services.AddMassTransit(bus =>
			{
				bus.SetKebabCaseEndpointNameFormatter();
				
				string? uri = configuration["RabbitMQ:Uri"];
				string? username = configuration["RabbitMQ:Username"];
				string? password = configuration["RabbitMQ:Password"];

				if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
				{
					Console.WriteLine("Config values not set for Rabbit URI, Username, or Password");
					Console.WriteLine("Exiting....");
					Environment.Exit(1);
				}
				
				bus.UsingRabbitMq((context, cfg) =>
				{
					cfg.Host(uri, "/", h => {
						h.Username(username);
						h.Password(password);
					});

					cfg.ConfigureEndpoints(context);
				});
			});
			
			services.AddMediatR(cfg =>
			{
				cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
			});
			
			services.AddPersistenceServices();
			services.AddPayPalServices(configuration);
		}
	}
}