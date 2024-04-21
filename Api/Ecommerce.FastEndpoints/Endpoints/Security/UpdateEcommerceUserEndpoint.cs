using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.EcommerceUser.Commands.UpdateEcommerceUser;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Security
{
	/// <summary>
	/// A Fast Endpoint implementation that handles updating an existing EcommerceUser
	/// </summary>
	public class UpdateEcommerceUserEndpoint : Endpoint<UpdateEcommerceUserRequest, UpdateEcommerceUserResponse>
	{
		private readonly ILogger<UpdateEcommerceUserEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;
		private readonly IAuthenticationService _authenticationService;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateEcommerceUserEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for token validation</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public UpdateEcommerceUserEndpoint(ILogger<UpdateEcommerceUserEndpoint> logger, IMediator mediator, IAuthenticationService authenticationService
			, ITokenService tokenService)
		{
			this._logger = logger;
			this._mediator = mediator;
			this._tokenService = tokenService;
			this._authenticationService = authenticationService;
		}
		
		/// <summary>
		/// Configures the route and roles for the Endpoint
		/// </summary>
		public override void Configure()
		{
			this.Put("/api/user/update");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="UpdateEcommerceUserRequest"/> and generates a <see cref="UpdateEcommerceUserResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="UpdateEcommerceUserRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(UpdateEcommerceUserRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Update User Request");
			
			//Check if token is valid
			if (await this._tokenService.ValidateTokenAsync(this.HttpContext.Request.Headers.Authorization) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}
			
			//Get the userid of the user if it exists
			Guid? userId = await this._authenticationService.GetUserIdByName(req.UserName ?? string.Empty);

			//Check if the user was found
			if (userId == null)
			{
				await this.SendOkAsync(new UpdateEcommerceUserResponse 
					{ Success = false, Message = EcommerceUserConstants._updateUserErrorMessage }, 
					ct);
				return;
			}
			
			UpdateEcommerceUserResponse response;
			try
			{
				//Send the update command
				response = await this._mediator.Send(new UpdateEcommerceUserCommand
				{
					UserId = (Guid)userId,
					UserName = req.UpdateUserName,
					FirstName = req.FirstName,
					LastName = req.LastName,
					Email = req.Email
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to update a user");
				await this.SendAsync(new UpdateEcommerceUserResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response
			await this.SendOkAsync(response, ct);
		}
	}
}