using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Security
{
	public class LoginEndpoint : Endpoint<AuthenticationRequest>
	{
		private readonly ILogger<LoginEndpoint> _logger;
		private readonly IAuthenticationService _authenticationService;

		public LoginEndpoint(ILogger<LoginEndpoint> logger, IAuthenticationService authenticationService)
		{
			this._logger = logger;
			this._authenticationService = authenticationService;
		}

		public override void Configure()
		{
			Post("api/login");
			AllowAnonymous();
		}

		public override async Task HandleAsync(AuthenticationRequest req, CancellationToken ct)
		{
			AuthenticatedUserModel? result = await this._authenticationService.AuthenticateAsync(req);

			if (result == null)
			{
				await SendAsync(400, cancellation: ct);
				return;
			}

			await SendAsync(result, 200, cancellation: ct);
		}
	}
}