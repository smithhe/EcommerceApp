using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Security
{
	public class RegisterEndpoint : Endpoint<CreateUserRequest>
	{
		private readonly ILogger<RegisterEndpoint> _logger;
		private readonly IAuthenticationService _authenticationService;

		public RegisterEndpoint(ILogger<RegisterEndpoint> logger, IAuthenticationService authenticationService)
		{
			this._logger = logger;
			this._authenticationService = authenticationService;
		}
		
		public override void Configure()
		{
			Post("api/register");
			AllowAnonymous();
		}

		public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
		{
			bool success = await this._authenticationService.CreateUserAsync(req);

			if (success)
			{
				await SendAsync(201, cancellation: ct);
				return;
			}

			await SendAsync(400, cancellation: ct);
		}
	}
}