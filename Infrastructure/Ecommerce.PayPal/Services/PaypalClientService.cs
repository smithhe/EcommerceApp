using System.Net;
using Ecommerce.PayPal.Contracts;
using Ecommerce.PayPal.Contracts.Refit;
using Ecommerce.Shared.Requests.PayPal;
using Ecommerce.Shared.Responses.PayPal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Refit;

namespace Ecommerce.PayPal.Services
{
    /// <summary>
    /// Service for interacting with the PayPal API
    /// </summary>
    public class PaypalClientService : IPaypalClientService
    {
        private readonly ILogger<PaypalClientService> _logger;
        private readonly ITokenCacheService _tokenCacheService;
        private readonly ITokenService _tokenService;
        private readonly IPayPalApiService _payPalApiService;
        private readonly string _payPalCacheKey;
        
        /// <summary>
        /// Constructor for the PayPal Client Service
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tokenCacheService"></param>
        /// <param name="tokenService"></param>
        /// <param name="payPalApiService"></param>
        /// <param name="configuration"></param>
        public PaypalClientService(ILogger<PaypalClientService> logger, ITokenCacheService tokenCacheService, ITokenService tokenService, 
            IPayPalApiService payPalApiService, IConfiguration configuration)
        {
            this._logger = logger;
            this._tokenCacheService = tokenCacheService;
            this._tokenService = tokenService;
            this._payPalApiService = payPalApiService;
            this._payPalCacheKey = configuration["Paypal:CacheKey"]!;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreatePayPalOrderResponse> CreateOrder(CreatePayPalOrderApiRequest request)
        {
            //Create a new Guid for the PayPal request to help ensure Idempotency
            Guid payPalRequestId = Guid.NewGuid();
            
            //Log the request
            this._logger.LogInformation($"Creating PayPal Order: {payPalRequestId}");
            
            //Send the create order request to PayPal
            ApiResponse<CreatePayPalOrderResponse> response = await this._payPalApiService.CreatePayPalOrder(payPalRequestId.ToString(), request);

            //Check if the response is unauthorized
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                //Generate a new token
                await this._tokenService.GetNewToken();
                
                //Send the create order request to PayPal again
                response = await this._payPalApiService.CreatePayPalOrder(payPalRequestId.ToString(), request);
            }

            //Check if the response is successful
            if (response.IsSuccessStatusCode)
            {
                return response.Content;
            }

            //Check if the response has an error message
            if (string.IsNullOrEmpty(response.Error.Content) == false)
            {
                this._logger.LogError(response.Error.Content);
            }

            //Return a failed response
            return new CreatePayPalOrderResponse
            {
                Success = false,
                Message = "Failed to create PayPal Order"
            };
        }
        
        
    }
}