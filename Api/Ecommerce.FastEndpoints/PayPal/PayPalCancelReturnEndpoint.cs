using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.PayPal.Commands.CancelPayPalOrder;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.PayPal
{
    /// <summary>
    /// A Fast Endpoint implementation that handles a cancel return from PayPal
    /// </summary>
    public class PayPalCancelReturnEndpoint : EndpointWithoutRequest
    {
        private readonly ILogger<PayPalCancelReturnEndpoint> _logger;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalCancelReturnEndpoint"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
        public PayPalCancelReturnEndpoint(ILogger<PayPalCancelReturnEndpoint> logger, IMediator mediator, IConfiguration configuration)
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
            this.Get("/api/paypal/checkout/cancel/{returnKey}");
            this.AllowAnonymous();
        }

        /// <summary>
        /// Handles the PayPal cancel return
        /// </summary>
        /// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        public override async Task HandleAsync(CancellationToken ct)
        {
            //Log the request to cancel the PayPal return
            this._logger.LogInformation("Handling request to cancel an order from PayPal");
            
            //Get the UI Url from the configuration
            string? uiUrl = this._configuration["Paypal:UIUrl"];
            
            //Get the return key from the route
            string? returnKey = this.Route<string>("returnKey");
            
            //Check if we have a return key
            if (string.IsNullOrEmpty(returnKey))
            {
                //Redirect to the error page in the UI
                //TODO: Update this to the correct error page when it is created
                await this.SendRedirectAsync($"{uiUrl}/error", cancellation: ct);
                return;
            }
            
            //Cancel the PayPal order
            bool success = await this._mediator.Send(new CancelPayPalOrderCommand { ReturnKey = returnKey }, ct);
            
            //Check if the cancel was successful
            if (success == false)
            {
                //Redirect to the error page in the UI
                //TODO: Update this to the correct error page when it is created
                await this.SendRedirectAsync($"{uiUrl}/error", cancellation: ct);
            }
            else
            {
                //Redirect to the success page in the UI
                await this.SendRedirectAsync($"{uiUrl}/checkout/cancel", cancellation: ct);
            }
        }
    }
}