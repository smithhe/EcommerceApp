using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Order.Queries.GetOrderAfterSuccessfulCheckout;
using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Requests.Order;
using Ecommerce.Shared.Responses.Order;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Order
{
    /// <summary>
    /// A Fast Endpoint implementation that handles getting an Order after a successful checkout
    /// </summary>
    public class GetOrderAfterSuccessfulCheckoutEndpoint : Endpoint<GetOrderAfterSuccessfulCheckoutApiRequest, GetOrderAfterSuccessfulCheckoutResponse>
    {
        private readonly ILogger<GetOrderAfterSuccessfulCheckoutEndpoint> _logger;
        private readonly IMediator _mediator;
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetOrderAfterSuccessfulCheckoutEndpoint"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        /// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for token validation</param>
        public GetOrderAfterSuccessfulCheckoutEndpoint(ILogger<GetOrderAfterSuccessfulCheckoutEndpoint> logger, IMediator mediator, IAuthenticationService authenticationService)
        {
            this._logger = logger;
            this._mediator = mediator;
            this._authenticationService = authenticationService;
        }
        
        /// <summary>
        /// Configures the route and roles for the Endpoint
        /// </summary>
        public override void Configure()
        {
            Get("/api/checkout/order/{id}");
            //TODO: Add roles
        }

        /// <summary>
        /// Handles the <see cref="GetOrderAfterSuccessfulCheckoutApiRequest"/> and generates a <see cref="GetOrderAfterSuccessfulCheckoutResponse"/>
        /// </summary>
        /// <param name="req">The <see cref="GetOrderAfterSuccessfulCheckoutApiRequest"/> object sent in the HTTP request</param>
        /// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        public override async Task HandleAsync(GetOrderAfterSuccessfulCheckoutApiRequest req, CancellationToken ct)
        {
            //Log the request
            this._logger.LogInformation("Handling request to get the Order after a successful checkout");
            
            //Check if token is valid
            string? token = this.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (await TokenService.ValidateTokenAsync(this._authenticationService, token) == false)
            {
                //Token is Invalid
                await this.SendUnauthorizedAsync(ct);
                return;
            }

            GetOrderAfterSuccessfulCheckoutResponse response;
            try
            {
                //Lookup the Order
                response = await this._mediator.Send(new GetOrderAfterSuccessfulCheckoutQuery { Id = req.Id }, ct);
            }
            catch (Exception ex)
            {
                //Unexpected error
                this._logger.LogError(ex, "Error when attempting to get a Order after a successful checkout");
                await this.SendAsync(new GetOrderAfterSuccessfulCheckoutResponse { Success = false, Message = "Unexpected Error Occurred" },
                    500, ct);
                return;
            }
            
            //Send the response
            await this.SendAsync(response, cancellation: ct);
        }
    }
}