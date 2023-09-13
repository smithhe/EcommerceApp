using Ecommerce.Application.Features.CartItem.Commands.UpdateCartItem;
using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Requests.CartItem;
using Ecommerce.Shared.Responses.CartItem;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.CartItem
{
	/// <summary>
	/// A Fast Endpoint implementation that handles updating an existing CartItem
	/// </summary>
	public class UpdateCartItemEndpoint : Endpoint<UpdateCartItemApiRequest, UpdateCartItemResponse>
	{
		private readonly ILogger<UpdateCartItemEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly IAuthenticationService _authenticationService;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateCartItemEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for token validation</param>
		public UpdateCartItemEndpoint(ILogger<UpdateCartItemEndpoint> logger, IMediator mediator, IAuthenticationService authenticationService)
		{
			this._logger = logger;
			this._mediator = mediator;
			this._authenticationService = authenticationService;
		}
		
		/// <summary>
		/// Configures the route and roles for the Endpoint
		/// </summary>
		public override void Configure()
		{
			Post("/api/cartitem/update");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="UpdateCartItemApiRequest"/> and generates a <see cref="UpdateCartItemResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="UpdateCartItemApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(UpdateCartItemApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Update CartItem Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
			if (await TokenService.ValidateTokenAsync(this._authenticationService, token) == false)
			{
				//Token is Invalid
				await SendUnauthorizedAsync(ct);
				return;
			}

			UpdateCartItemResponse response;
			try
			{
				//Send the update command
				response = await this._mediator.Send(new UpdateCartItemCommand
				{
					CartItemToUpdate = req.CartItemToUpdate, 
					UserName = TokenService.GetUserNameFromToken(token)
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to update CartItem");
				await SendAsync(new UpdateCartItemResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response object
			await SendOkAsync(response, ct);
		}
	}
}