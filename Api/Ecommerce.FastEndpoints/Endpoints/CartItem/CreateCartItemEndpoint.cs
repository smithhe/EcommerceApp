using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.CartItem.Commands.CreateCartItem;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.CartItem;
using Ecommerce.Shared.Responses.CartItem;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.CartItem
{
	/// <summary>
	/// A Fast Endpoint implementation that handles creating a new CartItem
	/// </summary>
	public class CreateCartItemEndpoint : Endpoint<CreateCartItemApiRequest, CreateCartItemResponse>
	{
		private readonly ILogger<CreateCartItemEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateCartItemEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public CreateCartItemEndpoint(ILogger<CreateCartItemEndpoint> logger, IMediator mediator, ITokenService tokenService)
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
			this.Post("/api/cartitem/create");
			//TODO: Add roles
		}
		
		/// <summary>
		/// Handles the <see cref="CreateCartItemApiRequest"/> and generates a <see cref="CreateCartItemResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="CreateCartItemApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(CreateCartItemApiRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Create CartItem Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers.Authorization;
			if (await this._tokenService.ValidateTokenAsync(token) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}

			CreateCartItemResponse response;
			try
			{
				//Send the create command
				response = await this._mediator.Send(new CreateCartItemCommand
				{
					CartItemToCreate = req.CartItemToCreate, 
					UserName = this._tokenService.GetUserNameFromToken(token)
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to create CartItem");
				await this.SendAsync(new CreateCartItemResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}
			
			//Send the response
			await this.SendOkAsync(response, ct);
		}
	}
}