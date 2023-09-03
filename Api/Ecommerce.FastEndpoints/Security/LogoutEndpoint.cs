using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Security
{
	public class LogoutEndpoint : Endpoint<AuthenticatedUserModel>
	{
		private readonly ILogger<LogoutEndpoint> _logger;
		private readonly IAuthenticationService _authenticationService;

		public LogoutEndpoint(ILogger<LogoutEndpoint> logger, IAuthenticationService authenticationService)
		{
			this._logger = logger;
			this._authenticationService = authenticationService;
		}
		
		public override void Configure()
		{
			Post("api/logout");
		}

		public override async Task HandleAsync(AuthenticatedUserModel req, CancellationToken ct)
		{
			await this._authenticationService.LogoutAsync(req);

			await SendOkAsync(ct);
		}
	}
}