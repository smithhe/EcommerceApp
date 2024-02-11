namespace Ecommerce.PayPal.Contracts
{
    public interface ITokenService
    {
        Task<string> GetNewToken();
    }
}