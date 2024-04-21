using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.PayPal.Commands.HandlePayPalSuccess;
using Ecommerce.Application.Features.PayPal.Queries.GetOrderByReturnKey;
using Ecommerce.Shared.Dtos;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.PayPal
{
    /// <summary>
    /// A Fast Endpoint implementation that handles a successful return from placing an order with PayPal
    /// </summary>
    public class PayPalSuccessReturnEndpoint : EndpointWithoutRequest
    {
        private readonly ILogger<PayPalSuccessReturnEndpoint> _logger;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalSuccessReturnEndpoint"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
        public PayPalSuccessReturnEndpoint(ILogger<PayPalSuccessReturnEndpoint> logger, IMediator mediator, IConfiguration configuration)
        {
            this._logger = logger;
            this._mediator = mediator;
            this._configuration = configuration;
        }
        
        /// <summary>
        /// Configures the route and roles for the Endpoint
        /// </summary>
        public override void Configure()
        {
            this.Get("/api/paypal/checkout/success/{returnKey}");
            this.AllowAnonymous();
            this.Tags("PayPal");
        }

        /// <summary>
        /// Handles the PayPal success return
        /// </summary>
        /// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        public override async Task HandleAsync(CancellationToken ct)
        {
            //Log the request to handle the PayPal success return
            this._logger.LogInformation("Handling request to handle a successful return from PayPal");
            
            //Get the return key from the route
            string? returnKey = this.Route<string>("returnKey");
            
            //Get the UI Url from the configuration
            string? uiUrl = this._configuration["UIUrl"];
            
            //Check if we have a return key
            if (string.IsNullOrEmpty(returnKey))
            {
                //Redirect to the error page in the UI
                //TODO: Update this to the correct error page when it is created
                await this.SendRedirectAsync($"{uiUrl}/error");
                return;
            }
            
            //Handle the PayPal success return
            bool success = await this._mediator.Send(new HandlePayPalSuccessCommand { ReturnKey = returnKey }, ct);
            
            //Check if the handle was successful
            if (success == false)
            {
                //Redirect to the error page in the UI
                //TODO: Update this to the correct error page when it is created
                await this.SendRedirectAsync($"{uiUrl}/error");
                return;
            }
            
            //Get the Order from the PayPal return key
            OrderDto? order = await this._mediator.Send(new GetOrderByReturnKeyQuery { ReturnKey = returnKey }, ct);
            
            //Check if we have an order
            if (order == null)
            {
                //Log the error and return false
                this._logger.LogError("Failed to get the order from the return key");
                //TODO: Update this to the correct error page when it is created
                await this.SendRedirectAsync($"{uiUrl}/error");
                return;
            }
            
            //Redirect to the success page in the UI
            await this.SendRedirectAsync($"{uiUrl}/checkout/success/{order.Id}");
        }
    }
}