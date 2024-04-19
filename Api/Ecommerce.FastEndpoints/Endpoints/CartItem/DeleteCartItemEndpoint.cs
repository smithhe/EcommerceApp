using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.CartItem.Commands.DeleteCartItem;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.CartItem;
using Ecommerce.Shared.Responses.CartItem;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.CartItem
{
	/// <summary>
	/// A Fast Endpoint implementation that handles deleting a CartItem
	/// </summary>
	public class DeleteCartItemEndpoint : Endpoint<DeleteCartItemApiRequest, DeleteCartItemResponse>
	{
		private readonly ILogger<DeleteCartItemEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteCartItemEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public DeleteCartItemEndpoint(ILogger<DeleteCartItemEndpoint> logger, IMediator mediator, ITokenService tokenService)
		{
			this._logger = logger;
			this._mediator = mediator;
			this._tokenService = tokenService;
		}
		
		/// <summary>
		/// Configures the route and roles for the Endpoint
		/// </summary>
		public override void Configure()
		{
			this.Delete("/api/cartitem/delete");
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
			if (await this._tokenService.ValidateTokenAsync(this.HttpContext.Request.Headers.Authorization) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
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
				await this.SendAsync(new DeleteCartItemResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response object
			await this.SendOkAsync(response, ct);
		}
	}
}