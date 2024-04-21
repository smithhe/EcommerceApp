using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.EcommerceUser.Commands.ConfirmEmail;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Security
{
    /// <summary>
    /// A Fast Endpoint implementation that handles confirming a User's email address
    /// </summary>
    public class ConfirmEmailEndpoint : Endpoint<ConfirmEmailRequest, ConfirmEmailResponse>
    {
        private readonly ILogger<ConfirmEmailEndpoint> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmEmailEndpoint"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        public ConfirmEmailEndpoint(ILogger<ConfirmEmailEndpoint> logger, IMediator mediator)
        {
            this._logger = logger;
            this._mediator = mediator;
        }
        
        /// <summary>
        /// Configures the route for the Endpoint
        /// </summary>
        public override void Configure()
        {
            this.Post("/api/user/confirm-email");
            this.AllowAnonymous();
            this.Options(o => o.WithTags("Security"));
        }

        /// <summary>
        /// Handles the <see cref="ConfirmEmailRequest"/> and generates a <see cref="ConfirmEmailResponse"/>
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ct"></param>
        public override async Task HandleAsync(ConfirmEmailRequest req, CancellationToken ct)
        {
            //Log the request
            this._logger.LogInformation("Handling request to confirm a User's email address");

            ConfirmEmailResponse response;
            try
            {
                //Attempt to confirm the user's email
                response = await this._mediator.Send(new ConfirmEmailCommand
                {
                    UserId = req.UserId ?? string.Empty,
                    Token = req.EmailToken ?? string.Empty
                }, ct);
            }
            catch (Exception e)
            {
                //Log the exception
                this._logger.LogError(e, "An error occurred while registering a new User");
                await this.SendAsync(new ConfirmEmailResponse { Success = false, Message = "Unexpected Error Occurred" },
                    500, ct);
                return;
            }
            
            //Send the response
            await this.SendAsync(response, 200, ct);
        }
    }
}