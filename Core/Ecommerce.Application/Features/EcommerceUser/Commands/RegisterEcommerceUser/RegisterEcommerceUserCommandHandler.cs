using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Identity.Contracts;
using Ecommerce.Messages.EcommerceUser;
using Ecommerce.Shared.Security.Responses;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.EcommerceUser.Commands.RegisterEcommerceUser
{
    /// <summary>
    /// A <see cref="Mediator"/> request handler for a <see cref="RegisterEcommerceUserCommand"/>
    /// </summary>
    public class RegisterEcommerceUserCommandHandler : IRequestHandler<RegisterEcommerceUserCommand, CreateUserResponse>
    {
        private readonly ILogger<RegisterEcommerceUserCommandHandler> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IConfiguration _configuration;
        private readonly IBus _bus;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterEcommerceUserCommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for registering the User</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
        /// <param name="bus">The <see cref="IBus"/> instance used for publishing messages</param>
        public RegisterEcommerceUserCommandHandler(ILogger<RegisterEcommerceUserCommandHandler> logger, IAuthenticationService authenticationService,
            IConfiguration configuration, IBus bus)
        {
            this._logger = logger;
            this._authenticationService = authenticationService;
            this._configuration = configuration;
            this._bus = bus;
        }
        
        /// <summary>
        /// Handles the <see cref="RegisterEcommerceUserCommand"/> command
        /// </summary>
        /// <param name="command">The <see cref="RegisterEcommerceUserCommand"/> request to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// Returns the <see cref="CreateUserResponse"/> generated from the <see cref="IAuthenticationService"/>
        /// </returns>
        public async Task<CreateUserResponse> Handle(RegisterEcommerceUserCommand command, CancellationToken cancellationToken)
        {
            //Log the request
            this._logger.LogInformation("Handling request to register a new User");
            
            //Attempt to register the user
            CreateUserResponse response = await this._authenticationService.CreateUserAsync(command.CreateUserRequest);

            //Check if the response was successful
            if (response.Success == false)
            {
                this._logger.LogError("Failed to create user, returning response");
                return response;
            }

            //Check if the Confirmation Link was null or empty
            if (string.IsNullOrEmpty(response.ConfirmationLink))
            {
                this._logger.LogError("Confirmation Token was null or empty, returning response");
                return response;
            }
            
            //Get the id of the new user
            Guid? id = await this._authenticationService.GetUserIdByName(command.CreateUserRequest.UserName!);
            
            //Check if the id is null
            if (id == null)
            {
                this._logger.LogError("Failed to get the new User's Id, returning response");
                return response;
            }
            
            //Update the response with the full URL
            string token = response.ConfirmationLink;
            response.ConfirmationLink = $"{command.LinkUrl}?userId={id}&emailToken={Uri.EscapeDataString(token)}";
            
            //Get the company name
            string? companyName = this._configuration["CompanyName"];

            //Send the message
            this._logger.LogInformation("Sending SendEmailConfirmationMessage to Rabbit");
            await this._bus.Publish(new SendEmailConfirmationMessage
            {
                ConfirmationLink = response.ConfirmationLink,
                CompanyName = companyName ?? string.Empty,
                Name = $"{command.CreateUserRequest.FirstName} {command.CreateUserRequest.LastName}",
                SendTo = command.CreateUserRequest.EmailAddress!
            }, cancellationToken);
            this._logger.LogInformation($"SendEmailConfirmationMessage Sent to Rabbit");

            //Return the response
            return response;
        }
    }
}