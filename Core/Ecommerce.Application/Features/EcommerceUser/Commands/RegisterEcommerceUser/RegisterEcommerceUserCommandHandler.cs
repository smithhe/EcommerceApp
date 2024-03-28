using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Identity.Contracts;
using Ecommerce.Messages.EcommerceUser;
using Ecommerce.Shared.Security;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterEcommerceUserCommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for registering the User</param>
        /// <param name="configuration"></param>
        public RegisterEcommerceUserCommandHandler(ILogger<RegisterEcommerceUserCommandHandler> logger, IAuthenticationService authenticationService,
            IConfiguration configuration)
        {
            this._logger = logger;
            this._authenticationService = authenticationService;
            this._configuration = configuration;
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
            
            //Update the response with the full URL
            response.ConfirmationLink = $"{command.LinkUrl}/{response.ConfirmationLink}";

            //Create a new Bus to send the message
            IBusControl? bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(this._configuration["RabbitMQ:Username"], h =>
                {
                    h.Username(this._configuration["RabbitMQ:Username"]);
                    h.Password(this._configuration["RabbitMQ:Password"]);
                });
            });
            
            //Check if the bus was created
            if (bus == null)
            {
                this._logger.LogError("Bus was null, returning response");
                return response;
            }

            //Start the bus
            await bus.StartAsync(cancellationToken);
			
            //Send the message
            this._logger.LogInformation("Sending message");
            await bus.Publish(new SendEmailConfirmationMessage
            {
                ConfirmationLink = response.ConfirmationLink,
                Name = $"{command.CreateUserRequest.FirstName} {command.CreateUserRequest.LastName}",
                SendTo = command.CreateUserRequest.EmailAddress!
            }, cancellationToken);
            this._logger.LogInformation($"Message Sent to Rabbit");

            //Return the response
            return response;
        }
    }
}