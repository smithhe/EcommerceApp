using System.Net.Http.Headers;
using Ecommerce.PayPal.Contracts;
using Ecommerce.PayPal.Contracts.Refit;
using Ecommerce.PayPal.Security;
using Ecommerce.PayPal.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Ecommerce.PayPal
{
    public static class PayPalServiceRegistration
    {
        public static void AddPayPalServices(this IServiceCollection services, IConfiguration configuration)
        {
            string payPalApiEndpoint = configuration["Paypal:BaseUrl"] ?? "https://api-m.sandbox.paypal.com";
            string? clientId = configuration["Paypal:ClientId"];
            string? secret = configuration["Paypal:Secret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(secret))
            {
                throw new Exception(
                    "PayPal ClientId and Secret are required. Please add them to the appsettings.json file.");
            }

            services.AddMemoryCache();
            
            services.AddScoped<IPaypalClientService, PaypalClientService>();
            services.AddScoped<ITokenCacheService, TokenCacheService>();
            services.AddScoped<ITokenService, TokenService>();
            
            services.AddHttpClient<TokenService>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(payPalApiEndpoint);
                    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    
                    string clientCredentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{secret}"));
                    c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", clientCredentials);
                });

            services.AddRefitClient<IPayPalApiService>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(payPalApiEndpoint);
                }).AddHttpMessageHandler<AuthHeaderHandler>();
        }
    }
}