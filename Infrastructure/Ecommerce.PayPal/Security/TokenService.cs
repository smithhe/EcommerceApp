using System.Text.Json;
using Ecommerce.PayPal.Contracts;
using Ecommerce.PayPal.Models.Responses;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.PayPal.Security
{
    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenCacheService _tokenCacheService;
        private readonly string _payPalCacheKey;
        
        public TokenService(HttpClient httpClient, ITokenCacheService tokenCacheService, IConfiguration configuration)
        {
            this._httpClient = httpClient;
            this._tokenCacheService = tokenCacheService;
            this._payPalCacheKey = configuration["Paypal:CacheKey"]!;
        }

        public async Task<string> GetNewToken()
        {
            //Create the request data
            FormUrlEncodedContent requestData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            //Send the request to PayPal
            HttpResponseMessage response = await this._httpClient.PostAsync("/v1/oauth2/token", requestData);

            //Check if the request was successful
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to obtain the PayPal token. Status code: {response.StatusCode}");
            }

            //Read the response
            string jsonResponse = await response.Content.ReadAsStringAsync();
            PayPalAuthTokenResponse? tokenResponse = JsonSerializer.Deserialize<PayPalAuthTokenResponse>(jsonResponse);
            
            //Check if the response was empty
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new HttpRequestException("Failed to obtain the PayPal token. The response was empty.");
            }
            
            //Set the token in the cache
            this._tokenCacheService.SetToken(this._payPalCacheKey, tokenResponse.AccessToken, TimeSpan.FromSeconds(tokenResponse.ExpiresIn));
            
            //Return the token
            return tokenResponse.AccessToken;
        }
    }
}