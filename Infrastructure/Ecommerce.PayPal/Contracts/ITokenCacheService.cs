namespace Ecommerce.PayPal.Contracts
{
    public interface ITokenCacheService
    {
        void SetToken(string key, string token, TimeSpan expiresIn);
        string? GetToken(string key);
    }
}