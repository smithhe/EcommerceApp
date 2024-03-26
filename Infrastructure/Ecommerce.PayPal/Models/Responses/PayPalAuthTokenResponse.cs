using System.Text.Json.Serialization;

namespace Ecommerce.PayPal.Models.Responses
{
    /// <summary>
    /// Represents the response from PayPal when requesting an auth token.
    /// </summary>
    public class PayPalAuthTokenResponse
    {
        /// <summary>
        /// The scope of the token.
        /// </summary>
        [JsonPropertyName("scope")]
        public string? Scope { get; set; }
        
        /// <summary>
        /// The access token to use for requests.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
        
        /// <summary>
        /// The token type.
        /// </summary>
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
        
        /// <summary>
        /// The app id.
        /// </summary>
        [JsonPropertyName("app_id")]
        public string? AppId { get; set; }
        
        /// <summary>
        /// Time in seconds until the token expires.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        /// <summary>
        /// The nonce.
        /// </summary>
        [JsonPropertyName("nonce")]
        public string? Nonce { get; set; }
    }
}