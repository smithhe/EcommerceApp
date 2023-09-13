using Ecommerce.Application.Features.CartItem.Commands.CreateCartItem;
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
	/// A Fast Endpoint implementation that handles creating a new CartItem
	/// </summary>
	public class CreateCartItemEndpoint : Endpoint<CreateCartItemApiRequest, CreateCartItemResponse>
	{
		private readonly ILogger<CreateCartItemEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly IAuthenticationService _authenticationService;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for token validation</param>
		public CreateCartItemEndpoint(ILogger<CreateCartItemEndpoint> logger, IMediator mediator, IAuthenticationService authenticationService)
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
			Post("/api/cartitem/create");
			//TODO: Add roles
		}
		
		/// <summary>
		/// Handles the <see cref="CreateCartItemApiRequest"/> and generates a <see cref="CreateCartItemResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="CreateCartItemApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(CreateCartItemApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Create CartItem Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
			if (await TokenService.ValidateTokenAsync(this._authenticationService, token) == false)
			{
				//Token is Invalid
				await SendUnauthorizedAsync(ct);
				return;
			}

			CreateCartItemResponse response;

			try
			{
				//Send the create command
				response = await this._mediator.Send(new CreateCartItemCommand
				{
					CartItemToCreate = req.CartItemToCreate, 
					UserName = TokenService.GetUserNameFromToken(token)
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempt to create CartItem");
				await SendAsync(new CreateCartItemResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response object
			await SendOkAsync(response, ct);
		}
	}
}