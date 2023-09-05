using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Security
{
	public class RegisterEndpoint : Endpoint<CreateUserRequest, CreateUserResponse>
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
			Post("/api/register");
			AllowAnonymous();
		}

		public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
		{
			CreateUserResponse response = await this._authenticationService.CreateUserAsync(req);

			if (response.Success)
			{
				await SendAsync(response, 201, cancellation: ct);
				return;
			}

			await SendAsync(response, 400, cancellation: ct);
		}
	}
}