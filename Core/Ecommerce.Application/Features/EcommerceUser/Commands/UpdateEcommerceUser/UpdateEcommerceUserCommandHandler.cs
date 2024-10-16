using Ecommerce.Identity.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.Shared.Security.Responses;

namespace Ecommerce.Application.Features.EcommerceUser.Commands.UpdateEcommerceUser
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="UpdateEcommerceUserCommand"/>
	/// </summary>
	public class UpdateEcommerceUserCommandHandler : IRequestHandler<UpdateEcommerceUserCommand, UpdateEcommerceUserResponse>
	{
		private readonly ILogger<UpdateEcommerceUserCommandHandler> _logger;
		private readonly IAuthenticationService _authenticationService;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateEcommerceUserCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for updating the user.</param>
		public UpdateEcommerceUserCommandHandler(ILogger<UpdateEcommerceUserCommandHandler> logger, IAuthenticationService authenticationService)
		{
			this._logger = logger;
			this._authenticationService = authenticationService;
		}

		/// <summary>
		/// Handles the <see cref="UpdateEcommerceUserCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="UpdateEcommerceUserCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="UpdateEcommerceUserResponse"/> with Success being <c>true</c> if the <see cref="EcommerceUser"/> was updated;
		/// Success will be <c>false</c> if no <see cref="EcommerceUser"/> is found or validation of the command fails.
		/// Message will contain the message to display to the user.
		/// Validation Errors will be populated with errors to present if validation fails
		/// </returns>
		public async Task<UpdateEcommerceUserResponse> Handle(UpdateEcommerceUserCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to update a User's information");
			
			//Create the response object
			UpdateEcommerceUserResponse response = new UpdateEcommerceUserResponse { Success = true, Message = EcommerceUserConstants._updateUserSuccessMessage };

			//Check for null or empty properties in the command
			if (string.IsNullOrEmpty(command.UserName) || string.IsNullOrEmpty(command.Email) ||
			    string.IsNullOrEmpty(command.FirstName) || string.IsNullOrEmpty(command.LastName))
			{
				response.Success = false;
				response.Message = EcommerceUserConstants._updateUserErrorMessage;
				return response;
			}

			//Check that the user exists
			Domain.Entities.EcommerceUser? existingUser = await this._authenticationService.GetUserById(command.UserId);
			if (existingUser == null)
			{
				response.Success = false;
				response.Message = EcommerceUserConstants._updateUserErrorMessage;
				return response;
			}

			//Update the user from the information in the command
			existingUser.FirstName = command.FirstName;
			existingUser.LastName = command.LastName;
			existingUser.Email = command.Email;

			//Return the response from the update attempt
			return await this._authenticationService.UpdateUser(existingUser, command.UserName);
		}
	}
}