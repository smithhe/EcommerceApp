using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.CartItem.Queries.GetUserCartItems;
using Ecommerce.Domain.Constants.Identity;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.CartItem;
using Ecommerce.Shared.Responses.CartItem;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.CartItem
{
	/// <summary>
	/// A Fast Endpoint implementation that handles getting all CartItems for a User
	/// </summary>
	public class GetUserCartItemsEndpoint : Endpoint<GetUserCartItemsApiRequest, GetUserCartItemsResponse>
	{
		private readonly ILogger<GetUserCartItemsEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetUserCartItemsEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public GetUserCartItemsEndpoint(ILogger<GetUserCartItemsEndpoint> logger, IMediator mediator, ITokenService tokenService)
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
			this.Get("/api/cartitem/all");
			this.Policies(PolicyNames._generalPolicy);
		}

		/// <summary>
		/// Handles the <see cref="GetUserCartItemsApiRequest"/> and generates a <see cref="GetUserCartItemsResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="GetUserCartItemsApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(GetUserCartItemsApiRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Get All CartItems Request");
			
			//Check if token is valid
			if (await this._tokenService.ValidateTokenAsync(this.HttpContext.Request.Headers.Authorization) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}

			GetUserCartItemsResponse response;
			try
			{
				//Send the query
				response = await this._mediator.Send(new GetUserCartItemsQuery { UserId = req.UserId }, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to get all CartItems");
				await this.SendAsync(new GetUserCartItemsResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response
			await this.SendOkAsync(response, ct);
		}
	}
}