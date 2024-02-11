using Refit;

namespace Ecommerce.PayPal.Models.Responses
{
    public class PayPalAuthTokenResponse
    {
        [AliasAs("scope")]
        public string? Scope { get; set; }
        
        [AliasAs("access_token")]
        public string? AccessToken { get; set; }
        
        [AliasAs("token_type")]
        public string? TokenType { get; set; }
        
        [AliasAs("app_id")]
        public string? AppId { get; set; }
        
        [AliasAs("expires_in")]
        public int ExpiresIn { get; set; }
        
        [AliasAs("nonce")]
        public string? Nonce { get; set; }
    }
}