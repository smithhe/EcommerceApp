using System.Net.Http.Headers;
using Ecommerce.PayPal.Contracts;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.PayPal.Security
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ITokenCacheService _tokenCacheService;
        private readonly string _payPalCacheKey;

        public AuthHeaderHandler(ITokenCacheService tokenCacheService, IConfiguration configuration)
        {
            this._tokenCacheService = tokenCacheService;
            this._payPalCacheKey = configuration["Paypal:CacheKey"]!;
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? token = this._tokenCacheService.GetToken(this._payPalCacheKey);

            if (string.IsNullOrEmpty(token) == false)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
			
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}