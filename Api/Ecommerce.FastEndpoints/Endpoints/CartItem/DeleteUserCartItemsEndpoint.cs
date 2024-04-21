using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.CartItem.Commands.DeleteUserCartItems;
using Ecommerce.Domain.Constants.Identity;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.CartItem;
using Ecommerce.Shared.Responses.CartItem;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.CartItem
{
	/// <summary>
	/// A Fast Endpoint implementation that handles deleting a User's CartItems
	/// </summary>
	public class DeleteUserCartItemsEndpoint : Endpoint<DeleteUserCartItemsApiRequest, DeleteUserCartItemsResponse>
	{
		private readonly ILogger<DeleteUserCartItemsEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteUserCartItemsEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public DeleteUserCartItemsEndpoint(ILogger<DeleteUserCartItemsEndpoint> logger, IMediator mediator, ITokenService tokenService)
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
			this.Delete("/api/cartitem/user/delete");
			this.Policies(PolicyNames._generalPolicy);
			this.Options(o => o.WithTags("Cart Item"));
		}

		/// <summary>
		/// Handles the <see cref="DeleteUserCartItemsApiRequest"/> and generates a <see cref="DeleteUserCartItemsResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="DeleteUserCartItemsApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(DeleteUserCartItemsApiRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Delete User CartItems Request");
			
			//Check if token is valid
			if (await this._tokenService.ValidateTokenAsync(this.HttpContext.Request.Headers.Authorization) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}

			DeleteUserCartItemsResponse response;
			try
			{
				//Send the delete command
				response = await this._mediator.Send(new DeleteUserCartItemsCommand { UserId = req.UserId }, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to delete user CartItems");
				await this.SendAsync(new DeleteUserCartItemsResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response
			await this.SendOkAsync(response, ct);
		}
	}
}