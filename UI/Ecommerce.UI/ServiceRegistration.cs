using Blazored.LocalStorage;
using Blazored.Toast;
using Ecommerce.UI.Contracts;
using Ecommerce.UI.Contracts.Refit;
using Ecommerce.UI.Security;
using Ecommerce.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;

namespace Ecommerce.UI
{
	public static class ServiceRegistration
	{
		public static void AddProjectServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<ICategoryService, CategoryService>();
			services.AddScoped<IProductService, ProductService>();
			
			services.AddBlazoredToast();
			services.AddBlazoredLocalStorage();
			
			AddSecurityServices(services, configuration);
			AddRefit(services, configuration);
		}

		private static void AddSecurityServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<ISecurityService, SecurityService>();
			services.AddAuthorizationCore();
			services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
			services.AddTransient<AuthHeaderHandler>();
		}

		//https://github.com/reactiveui/refit
		private static void AddRefit(IServiceCollection services, IConfiguration configuration)
		{
			string apiEndpoint = configuration["ApiUri"] ?? "https://localhost:7205";
			
			services.AddRefitClient<ICategoryApiService>()
				.ConfigureHttpClient(c =>
				{
					c.BaseAddress = new Uri(apiEndpoint);
				}).AddHttpMessageHandler<AuthHeaderHandler>();
			
			services.AddRefitClient<IProductApiService>()
				.ConfigureHttpClient(c =>
				{
					c.BaseAddress = new Uri(apiEndpoint);
				}).AddHttpMessageHandler<AuthHeaderHandler>();
			
			services.AddRefitClient<ISecurityApiService>()
				.ConfigureHttpClient(c =>
				{
					c.BaseAddress = new Uri(apiEndpoint);
				}).AddHttpMessageHandler<AuthHeaderHandler>();
		}
	}
}