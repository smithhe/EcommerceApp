using Blazored.LocalStorage;
using Blazored.Modal;
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
			services.AddScoped<IReviewService, ReviewService>();
			services.AddScoped<ICartService, CartService>();
			services.AddScoped<IOrderService, OrderService>();
			
			//https://github.com/Blazored/Toast
			services.AddBlazoredToast();
			
			//https://github.com/Blazored/LocalStorage
			services.AddBlazoredLocalStorage();
			
			//https://github.com/Blazored/Modal
			services.AddBlazoredModal();
			
			AddSecurityServices(services);
			AddRefit(services, configuration);
		}

		//Video series that includes going over .Net Identity in a real world context
		//https://www.youtube.com/watch?v=2c4p6RGtkps&list=PLLWMQd6PeGY0bEMxObA6dtYXuJOGfxSPx&index=57
		private static void AddSecurityServices(IServiceCollection services)
		{
			services.AddScoped<ISecurityService, SecurityService>();
			services.AddAuthorizationCore();
			services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
			services.AddTransient<AuthHeaderHandler>();
		}

		//https://github.com/reactiveui/refit
		private static void AddRefit(IServiceCollection services, IConfiguration configuration)
		{
			string apiEndpoint = configuration["ApiUri"] ?? "http://localhost:5128";
			Console.WriteLine(apiEndpoint);
			
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
			
			services.AddRefitClient<IReviewApiService>()
				.ConfigureHttpClient(c =>
				{
					c.BaseAddress = new Uri(apiEndpoint);
				}).AddHttpMessageHandler<AuthHeaderHandler>();
			
			services.AddRefitClient<ICartApiService>()
				.ConfigureHttpClient(c =>
				{
					c.BaseAddress = new Uri(apiEndpoint);
				}).AddHttpMessageHandler<AuthHeaderHandler>();
			
			services.AddRefitClient<IOrderApiService>()
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