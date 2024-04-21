using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.EcommerceUser.Commands.UpdatePassword;
using Ecommerce.Domain.Constants.Identity;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Security
{
    /// <summary>
    /// A Fast Endpoint implementation that handles updating an existing EcommerceUser's password
    /// </summary>
    public class UpdatePasswordEndpoint : Endpoint<UpdatePasswordRequest, UpdatePasswordResponse>
    {
        private readonly ILogger<UpdatePasswordEndpoint> _logger;
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePasswordEndpoint"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        /// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
        public UpdatePasswordEndpoint(ILogger<UpdatePasswordEndpoint> logger, IMediator mediator, ITokenService tokenService)
        {
            this._logger = logger;
            this._mediator = mediator;
            this._tokenService = tokenService;
        }
        
        /// <summary>
        /// Configures the route and roles for the Endpoint
        /// </summary>
        public override void Configure()
        {
            this.Put("/api/password/update");
            this.Policies(PolicyNames._generalPolicy);
            this.Options(o => o.WithTags("Security"));
        }

        /// <summary>
        /// Handles the <see cref="UpdatePasswordRequest"/> and generates a <see cref="UpdatePasswordResponse"/> 
        /// </summary>
        /// <param name="req">The <see cref="UpdatePasswordRequest"/> object sent in the HTTP request</param>
        /// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        public override async Task HandleAsync(UpdatePasswordRequest req, CancellationToken ct)
        {
            //Log the request
            this._logger.LogInformation("Handling Update User Password Request");
            
            //Check if token is valid
            if (this._tokenService.ValidateTokenAsync(this.HttpContext.Request.Headers.Authorization).Result == false)
            {
                //Token is Invalid
                await this.SendUnauthorizedAsync(ct);
                return;
            }

            UpdatePasswordResponse response;
            try
            {
                //Send the update password command
                response = await this._mediator.Send(new UpdatePasswordCommand
                {
                    UserName = req.UserName,
                    CurrentPassword = req.CurrentPassword,
                    NewPassword = req.NewPassword
                }, ct);
            }
            catch (System.Exception e)
            {
                //Unexpected Error
                this._logger.LogError(e, "Error Updating User Password");
                await this.SendAsync(new UpdatePasswordResponse { Success = false, Message = "Unexpected Error Occurred" },
                    500, ct);
                return;
            }
            
            //Send the response
            await this.SendOkAsync(response, ct);
        }
    }
}