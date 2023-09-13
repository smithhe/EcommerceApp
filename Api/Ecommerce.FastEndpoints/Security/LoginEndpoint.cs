using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Security
{
	/// <summary>
	/// A Fast Endpoint implementation that handles logging in a User
	/// </summary>
	public class LoginEndpoint : Endpoint<AuthenticationRequest, AuthenticatedUserModel?>
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
			Post("/api/login");
			AllowAnonymous();
		}

		/// <summary>
		/// Handles the <see cref="AuthenticationRequest"/> and generates a <see cref="AuthenticatedUserModel"/> if login was successful
		/// </summary>
		/// <param name="req">The <see cref="AuthenticationRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(AuthenticationRequest req, CancellationToken ct)
		{
			//Attempt a login
			AuthenticatedUserModel? result = await this._authenticationService.AuthenticateAsync(req);

			if (result == null)
			{
				//Login failed
				await SendAsync(null,400, cancellation: ct);
				return;
			}

			//Login succeeded
			await SendAsync(result, 200, cancellation: ct);
		}
	}
}