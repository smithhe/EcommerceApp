using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.EcommerceUser.Commands.ConfirmEmail
{
    /// <summary>
    /// A <see cref="Mediator"/> request handler for <see cref="ConfirmEmailCommand"/>
    /// </summary>
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ConfirmEmailResponse>
    {
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmEmailCommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for updating the user.</param>
        public ConfirmEmailCommandHandler(ILogger<ConfirmEmailCommandHandler> logger, IAuthenticationService authenticationService)
        {
            this._logger = logger;
            this._authenticationService = authenticationService;
        }
        
        /// <summary>
        /// Handles the <see cref="ConfirmEmailCommand"/> command
        /// </summary>
        /// <param name="command">The <see cref="ConfirmEmailCommand"/> request to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// Returns the <see cref="ConfirmEmailResponse"/> generated from the <see cref="IAuthenticationService"/>
        /// </returns>
        public async Task<ConfirmEmailResponse> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            //Log the request
            this._logger.LogInformation("Handling request to confirm a User's email address");
            
            //Attempt to confirm the user's email
            return await this._authenticationService.ConfirmEmailAsync(command.UserId.ToString(), command.Token);
        }
    }
}