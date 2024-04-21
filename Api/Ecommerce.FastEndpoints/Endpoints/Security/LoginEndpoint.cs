using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;
using FastEndpoints;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Security
{
	/// <summary>
	/// A Fast Endpoint implementation that handles logging in a User
	/// </summary>
	public class LoginEndpoint : Endpoint<AuthenticationRequest, AuthenticateResponse>
	{
		private readonly ILogger<LoginEndpoint> _logger;
		private readonly IAuthenticationService _authenticationService;

		/// <summary>
		/// Initializes a new instance of the <see cref="LoginEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for authenticating User credentials</param>
		public LoginEndpoint(ILogger<LoginEndpoint> logger, IAuthenticationService authenticationService)
		{
			this._logger = logger;
			this._authenticationService = authenticationService;
		}

		/// <summary>
		/// Configures the route for the Endpoint
		/// </summary>
		public override void Configure()
		{
			this.Post("/api/login");
			this.AllowAnonymous();
		}

		/// <summary>
		/// Handles the <see cref="AuthenticationRequest"/> and generates a <see cref="AuthenticatedUserModel"/> if login was successful
		/// </summary>
		/// <param name="req">The <see cref="AuthenticationRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(AuthenticationRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Login Request");
			
			//Attempt a login
			AuthenticateResponse response;
			try
			{
				response = await this._authenticationService.AuthenticateAsync(req);
			}
			catch (Exception e)
			{
				//Log the exception
				this._logger.LogError(e, "An error occurred while attempting to login a User");
				await this.SendAsync(new AuthenticateResponse { SignInResult = SignInResponseResult.UnexpectedError},
					500, ct);
				return;
			}

			//Check if the login failed
			if (response.SignInResult == SignInResponseResult.InvalidCredentials)
			{
				await this.SendUnauthorizedAsync(ct);
			}
			
			//Check if the account is locked or not allowed
			if (response.SignInResult == SignInResponseResult.AccountLocked || response.SignInResult == SignInResponseResult.AccountNotAllowed)
			{
				await this.SendForbiddenAsync(ct);
			}

			//Login succeeded
			await this.SendAsync(response, 200, cancellation: ct);
		}
	}
}