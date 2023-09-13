using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Security
{
	/// <summary>
	/// A Fast Endpoint implementation that handles registering a new User
	/// </summary>
	public class RegisterEndpoint : Endpoint<CreateUserRequest, CreateUserResponse>
	{
		private readonly ILogger<RegisterEndpoint> _logger;
		private readonly IAuthenticationService _authenticationService;

		/// <summary>
		/// Initializes a new instance of the <see cref="RegisterEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for registering the User</param>
		public RegisterEndpoint(ILogger<RegisterEndpoint> logger, IAuthenticationService authenticationService)
		{
			this._logger = logger;
			this._authenticationService = authenticationService;
		}
		
		/// <summary>
		/// Configures the route for the Endpoint
		/// </summary>
		public override void Configure()
		{
			Post("/api/register");
			AllowAnonymous();
		}

		/// <summary>
		/// Handles the <see cref="CreateUserRequest"/> and generates a <see cref="CreateUserResponse"/> if registration was successful
		/// </summary>
		/// <param name="req">The <see cref="CreateUserRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
		{
			//Attempt to register the user
			CreateUserResponse response = await this._authenticationService.CreateUserAsync(req);

			//Successful registration
			if (response.Success)
			{
				await SendAsync(response, 201, cancellation: ct);
				return;
			}

			//Failed registration
			await SendAsync(response, 400, cancellation: ct);
		}
	}
}