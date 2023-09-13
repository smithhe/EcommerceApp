using Ecommerce.Application.Features.CartItem.Commands.DeleteCartItem;
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
	/// A Fast Endpoint implementation that handles deleting a CartItem
	/// </summary>
	public class DeleteCartItemEndpoint : Endpoint<DeleteCartItemApiRequest, DeleteCartItemResponse>
	{
		private readonly ILogger<DeleteCartItemEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly IAuthenticationService _authenticationService;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteCartItemEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for token validation</param>
		public DeleteCartItemEndpoint(ILogger<DeleteCartItemEndpoint> logger, IMediator mediator, IAuthenticationService authenticationService)
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
			Delete("/api/cartitem/delete");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="DeleteCartItemApiRequest"/> and generates a <see cref="DeleteCartItemResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="DeleteCartItemApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(DeleteCartItemApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Delete CartItem Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
			if (await TokenService.ValidateTokenAsync(this._authenticationService, token) == false)
			{
				//Token is Invalid
				await SendUnauthorizedAsync(ct);
				return;
			}

			DeleteCartItemResponse response;
			try
			{
				//Send the delete command
				response = await this._mediator.Send(new DeleteCartItemCommand
				{
					CartItemToDelete = req.CartItemToDelete, 
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to delete CartItem");
				await SendAsync(new DeleteCartItemResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response object
			await SendOkAsync(response, ct);
		}
	}
}