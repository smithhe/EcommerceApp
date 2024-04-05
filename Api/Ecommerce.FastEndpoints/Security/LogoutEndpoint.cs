using Ecommerce.Identity.Contracts;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Shared.Security.Requests;

namespace Ecommerce.FastEndpoints.Security
{
	/// <summary>
	/// A Fast Endpoint implementation that handles logging out a User on the Server
	/// </summary>
	public class LogoutEndpoint : Endpoint<LogoutUserRequest>
	{
		private readonly ILogger<LogoutEndpoint> _logger;
		private readonly IAuthenticationService _authenticationService;

		/// <summary>
		/// Initializes a new instance of the <see cref="LogoutEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for authenticating User credentials</param>
		public LogoutEndpoint(ILogger<LogoutEndpoint> logger, IAuthenticationService authenticationService)
		{
			this._logger = logger;
			this._authenticationService = authenticationService;
		}
		
		/// <summary>
		/// Configures the route for the Endpoint
		/// </summary>
		public override void Configure()
		{
			Post("/api/logout");
		}

		/// <summary>
		/// Handles the <see cref="LogoutUserRequest"/> and logs the User out on the Server
		/// </summary>
		/// <param name="req">The <see cref="LogoutUserRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(LogoutUserRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Logout Request");
			
			//Check if token is valid
			string? token = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
			if (await TokenService.ValidateTokenAsync(this._authenticationService, token) == false)
			{
				//Token is Invalid
				await SendUnauthorizedAsync(ct);
				return;
			}
			
			//Check for a UserName
			if (string.IsNullOrEmpty(req.UserName))
			{
				await SendAsync(null, 400, ct);
				return;
			}
			
			//Logout the user
			await this._authenticationService.LogoutAsync(req.UserName);

			//Send success response
			await SendOkAsync(ct);
		}
	}
}