using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Security
{
	public class LogoutEndpoint : Endpoint<LogoutUserRequest>
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
			Post("/api/logout");
		}

		public override async Task HandleAsync(LogoutUserRequest req, CancellationToken ct)
		{
			
			//Check if token is valid
			string? token = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
			if (await TokenService.ValidateTokenAsync(this._authenticationService, token) == false)
			{
				//Token is Invalid
				await SendUnauthorizedAsync(ct);
				return;
			}
			
			if (string.IsNullOrEmpty(req.UserName))
			{
				await SendAsync(null, 400, ct);
				return;
			}
			
			await this._authenticationService.LogoutAsync(req.UserName);

			await SendOkAsync(ct);
		}
	}
}