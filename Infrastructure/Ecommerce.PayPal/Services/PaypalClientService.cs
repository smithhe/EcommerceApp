using System.Net;
using Ecommerce.Domain.Constants.Infrastructure;
using Ecommerce.PayPal.Contracts;
using Ecommerce.PayPal.Contracts.Refit;
using Ecommerce.PayPal.Models;
using Ecommerce.PayPal.Models.Enums;
using Ecommerce.PayPal.Models.Requests;
using Ecommerce.PayPal.Models.Responses;
using Ecommerce.Shared.Dtos;
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
        private readonly ITokenService _tokenService;
        private readonly IPayPalApiService _payPalApiService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for the PayPal Client Service
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="tokenService">The <see cref="ITokenService"/> service used for Auth Token operations</param>
        /// <param name="payPalApiService">The Refit contract for calling the PayPal API</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
        public PaypalClientService(ILogger<PaypalClientService> logger, ITokenService tokenService, 
            IPayPalApiService payPalApiService, IConfiguration configuration)
        {
            this._logger = logger;
            this._tokenService = tokenService;
            this._payPalApiService = payPalApiService;
            this._configuration = configuration;
        }
        
        /// <summary>
        /// Service method for creating a PayPal Order
        /// </summary>
        /// <param name="request">The Ecommerce Api Request to create an order</param>
        /// <returns>
        /// Returns true with the redirect url and the PayPal RequestId if the order was created successfully;
        /// Returns false if the order creation failed.
        /// Message will contain the message to display to the user.
        /// </returns>
        public async Task<CreatePayPalOrderResponse> CreateOrder(CreatePayPalOrderRequest request)
        {
            //Create the response object
            CreatePayPalOrderResponse response = new CreatePayPalOrderResponse
            {
                Success = true,
                Message = PayPalConstants._createOrderSuccessMessage
            };
            
            //Verify we have a order to create
            if (request.Order?.OrderItems == null || request.Order.OrderItems.Any() == false)
            {
                response.Success = false;
                response.Message = PayPalConstants._createOrderErrorMessage;
                return response;
            }
            
            //Log the request
            this._logger.LogInformation($"Creating PayPal Order: {request.Order.PayPalRequestId}");
            
            //Create the return urls
            string? baseReturnUrl = this._configuration["PayPal:ReturnBaseUrl"];

            if (string.IsNullOrEmpty(baseReturnUrl))
            {
                response.Success = false;
                response.Message = PayPalConstants._createOrderErrorMessage;
                return response;
            }
            
            string returnUrl = $"{baseReturnUrl}/api/paypal/checkout/success/{request.ReturnKey}";
            string cancelUrl = $"{baseReturnUrl}/api/paypal/checkout/cancel/{request.ReturnKey}";
            
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
                            BrandName = "TechGear Forge",
                            Locale = "en-US",
                            LandingPage = LandingPage.LOGIN,
                            ShippingPreference = ShippingPreference.NO_SHIPPING,
                            UserAction = UserAction.PAY_NOW,
                            ReturnUrl = returnUrl,
                            CancelUrl = cancelUrl
                        }
                    }
                }
            };
            
            //Create the items for the purchase unit
            List<Item> purchaseUnitItems = new List<Item>();
            foreach (OrderItemDto orderItem in request.Order.OrderItems)
            {
                purchaseUnitItems.Add(new Item
                {
                    Name = orderItem.ProductName,
                    Description = orderItem.ProductDescription,
                    Sku = orderItem.ProductSku,
                    UnitAmount = new Currency
                    {
                        CurrencyCode = "USD",
                        Value = orderItem.Price.ToString("0.00")
                    },
                    Quantity = orderItem.Quantity.ToString(),
                    Category = Category.DIGITAL_GOODS
                });
            }
            
            PurchaseUnit purchaseUnit = new PurchaseUnit
            {
                Amount = new PurchaseAmount
                {
                    CurrencyCode = "USD",
                    Value = request.Order.Total.ToString("0.00"),
                    BreakDown = new BreakDown
                    {
                        ItemTotal = new Currency
                        {
                            CurrencyCode = "USD",
                            Value = request.Order.Total.ToString("0.00")
                        }
                    }
                },
                Items = purchaseUnitItems.ToArray(),
                Description = "TechGear Forge Order",
                SoftDescriptor = "TechGear Forge Order",
            };
            
            //Add the purchase units to the request
            payPalCreateOrderRequest.PurchaseUnits = new PurchaseUnit[] { purchaseUnit };
            
            //Send the create order request to PayPal
            ApiResponse<PayPalCreateOrderResponse> payPalApiResponse = await this._payPalApiService.CreatePayPalOrder(request.Order.PayPalRequestId.ToString(), payPalCreateOrderRequest);

            //Check if the response is unauthorized
            if (payPalApiResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                //Generate a new token
                await this._tokenService.GetNewToken();
                
                //Send the create order request to PayPal again
                payPalApiResponse = await this._payPalApiService.CreatePayPalOrder(request.Order.PayPalRequestId.ToString(), payPalCreateOrderRequest);
            }
            
            //Check if the response has an error message
            if (string.IsNullOrEmpty(payPalApiResponse.Error?.Content) == false)
            {
                this._logger.LogError(payPalApiResponse.Error.Content);
            }

            //Check if the response is successful
            if (payPalApiResponse.IsSuccessStatusCode == false)
            {
                response.Success = false;
                response.Message = PayPalConstants._createOrderErrorMessage;
                return response;
            }
            
            //Get the response content
            PayPalCreateOrderResponse? responseContent = payPalApiResponse.Content;

            //Update the response object
            response.RedirectUrl = responseContent?.Links.FirstOrDefault(x => x.Rel == "payer-action")?.Href;

            //Return the response
            return response;
        }
    }
}