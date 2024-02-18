using System.Net;
using Ecommerce.PayPal.Contracts;
using Ecommerce.PayPal.Contracts.Refit;
using Ecommerce.PayPal.Models;
using Ecommerce.PayPal.Models.Enums;
using Ecommerce.PayPal.Models.Requests;
using Ecommerce.PayPal.Models.Responses;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.PayPal;
using Ecommerce.Shared.Responses.PayPal;
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
        private readonly ITokenService _tokenService;
        private readonly IPayPalApiService _payPalApiService;
        
        /// <summary>
        /// Constructor for the PayPal Client Service
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="tokenService">The <see cref="ITokenService"/> service used for Auth Token operations</param>
        /// <param name="payPalApiService">The Refit contract for calling the PayPal API</param>
        public PaypalClientService(ILogger<PaypalClientService> logger, ITokenService tokenService, IPayPalApiService payPalApiService)
        {
            this._logger = logger;
            this._tokenService = tokenService;
            this._payPalApiService = payPalApiService;
        }
        
        /// <summary>
        /// Service method for creating a PayPal Order
        /// </summary>
        /// <param name="request">The Ecommerce Api Request to create an order</param>
        /// <returns>
        /// Returns true with the redirect url and the PayPal RequestId if the order was created successfully;
        /// Returns false with a message if the order creation failed
        /// </returns>
        public async Task<CreatePayPalOrderResponse> CreateOrder(CreatePayPalOrderRequest request)
        {
            //Create the response object
            CreatePayPalOrderResponse response = new CreatePayPalOrderResponse
            {
                Success = false,
                Message = "Failed to create PayPal Order"
            };
            
            //Create a new Guid for the PayPal request to help ensure Idempotency
            Guid payPalRequestId = Guid.NewGuid();
            
            //Log the request
            this._logger.LogInformation($"Creating PayPal Order: {payPalRequestId}");
            
            //Verify we have a order to create
            if (request.Order == null)
            {
                response.Message = "No order provided to create a PayPal Order";
                return response;
            }
            
            //Create the request object
            PayPalCreateOrderRequest payPalCreateOrderRequest = new PayPalCreateOrderRequest
            {
                Intent = Intent.CAPTURE,
                PaymentSource = new PaymentSource
                {
                    PayPal = new Models.PayPal
                    {
                        ExperienceContext = new ExperienceContext
                        {
                            PaymentMethodPreference = PaymentMethodPreference.IMMEDIATE_PAYMENT_REQUIRED,
                            BrandName = "Ecommerce"
                        }
                    }
                }
            };
            
            List<PurchaseUnit> purchaseUnits = new List<PurchaseUnit>();
            
            //Add the purchase units to the request
            foreach (OrderItemDto orderItem in request.Order.OrderItems)
            {
                purchaseUnits.Add(new PurchaseUnit
                {
                    Amount = new Currency
                    {
                        CurrencyCode = "USD",
                        Value = orderItem.Price.ToString("F")
                    },
                    Items = new List<Item>
                    {
                        new Item
                        {
                            Name = orderItem.Name,
                            Description = orderItem.Description,
                            Sku = orderItem.Sku,
                            UnitAmount = new UnitAmount
                            {
                                CurrencyCode = "USD",
                                Value = orderItem.Price.ToString("F")
                            },
                            Quantity = orderItem.Quantity.ToString(),
                            Category = "PHYSICAL_GOODS"
                        }
                    }
                });
            }
            
            //Send the create order request to PayPal
            ApiResponse<PayPalCreateOrderResponse> payPalApiResponse = await this._payPalApiService.CreatePayPalOrder(payPalRequestId.ToString(), payPalCreateOrderRequest);

            //Check if the response is unauthorized
            if (payPalApiResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                //Generate a new token
                await this._tokenService.GetNewToken();
                
                //Send the create order request to PayPal again
                payPalApiResponse = await this._payPalApiService.CreatePayPalOrder(payPalRequestId.ToString(), payPalCreateOrderRequest);
            }

            //Check if the response is successful
            if (payPalApiResponse.IsSuccessStatusCode)
            {
                //Get the response content
                PayPalCreateOrderResponse? responseContent = payPalApiResponse.Content;

                //Update the response object
                response.Success = true;
                response.Message = "PayPal Order Created Successfully";
                response.PayPalRequestId = payPalRequestId;
                response.RedirectUrl = responseContent?.Links.FirstOrDefault(x => x.Rel == "approve")?.Href;

                //Return the response
                return response;
            }

            //Check if the response has an error message
            if (string.IsNullOrEmpty(payPalApiResponse.Error.Content) == false)
            {
                this._logger.LogError(payPalApiResponse.Error.Content);
            }

            //Return a failed response
            return response;
        }
        
        
    }
}