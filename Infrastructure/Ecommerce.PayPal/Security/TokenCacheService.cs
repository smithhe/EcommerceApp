using Ecommerce.PayPal.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.PayPal.Security
{
    public class TokenCacheService : ITokenCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _expirationBuffer = TimeSpan.FromMinutes(5);

        public TokenCacheService(IMemoryCache cache)
        {
            this._cache = cache;
        }

        public void SetToken(string key, string token, TimeSpan expiresIn)
        {
            // Subtract a buffer time to ensure token is refreshed before actual expiry
            TimeSpan expiration = expiresIn - this._expirationBuffer;
            this._cache.Set(key, token, expiration);
        }

        public string? GetToken(string key)
        {
            this._cache.TryGetValue(key, out string? token);
            return token;
        }
    }
}