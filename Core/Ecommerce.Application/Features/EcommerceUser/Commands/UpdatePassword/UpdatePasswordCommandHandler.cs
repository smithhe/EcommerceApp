using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.EcommerceUser.Commands.UpdatePassword
{
    /// <summary>
    /// A <see cref="Mediator"/> request handler for <see cref="UpdatePasswordCommand"/>
    /// </summary>
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, UpdatePasswordResponse>
    {
        private readonly ILogger<UpdatePasswordCommand> _logger;
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePasswordCommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for updating the user.</param>
        public UpdatePasswordCommandHandler(ILogger<UpdatePasswordCommand> logger, IAuthenticationService authenticationService)
        {
            this._logger = logger;
            this._authenticationService = authenticationService;
        }
        
        /// <summary>
        /// Handles the <see cref="UpdatePasswordCommand"/> request
        /// </summary>
        /// <param name="command">The <see cref="UpdatePasswordCommand"/> request to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// A <see cref="UpdateEcommerceUserResponse"/> with Success being <c>true</c> if the <see cref="EcommerceUser"/>'s password was updated;
        /// Success will be <c>false</c> if no <see cref="EcommerceUser"/> is found or validation of the command fails.
        /// Message will contain the error to display if Success is <c>false</c>;
        /// Validation Errors will be populated with errors to present if validation fails
        /// </returns>
        public async Task<UpdatePasswordResponse> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            //Log the request
            this._logger.LogInformation("Handling request to update a User's password");
            
            UpdatePasswordResponse response = new UpdatePasswordResponse();
            
            //Check for null or empty properties in the command
            if (string.IsNullOrEmpty(command.UserName) || string.IsNullOrEmpty(command.CurrentPassword) ||
                string.IsNullOrEmpty(command.NewPassword))
            {
                response.Success = false;
                response.Message = "All Fields are required";
                return response;
            }
            
            //Check that the user exists
            Guid? existingUserId = await this._authenticationService.GetUserIdByName(command.UserName);
            
            if (existingUserId == null)
            {
                response.Success = false;
                response.Message = "No User Found to Update";
                return response;
            }
            
            //Load in the existing user and update the password
            Domain.Entities.EcommerceUser? existingUser = await this._authenticationService.GetUserById(new Guid(existingUserId.ToString()!));
            return await this._authenticationService.UpdatePassword(existingUser, command.CurrentPassword, command.NewPassword);
        }
    }
}