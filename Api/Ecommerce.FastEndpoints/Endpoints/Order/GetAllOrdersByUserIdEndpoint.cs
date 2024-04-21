using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Order.Queries.GetAllOrdersByUserId;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.Order;
using Ecommerce.Shared.Responses.Order;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Order
{
	/// <summary>
	/// A Fast Endpoint implementation that handles getting all Orders for a User
	/// </summary>
	public class GetAllOrdersByUserIdEndpoint : Endpoint<GetAllOrdersByUserIdApiRequest, GetAllOrdersByUserIdResponse>
	{
		private readonly ILogger<GetAllOrdersByUserIdEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetAllOrdersByUserIdEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public GetAllOrdersByUserIdEndpoint(ILogger<GetAllOrdersByUserIdEndpoint> logger, IMediator mediator, ITokenService tokenService)
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
			this.Get("/api/order/user/all");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="GetAllOrdersByUserIdApiRequest"/> and generates a <see cref="GetAllOrdersByUserIdResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="GetAllOrdersByUserIdApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(GetAllOrdersByUserIdApiRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Get All Orders Request");
			
			//Check if token is valid
			if (await this._tokenService.ValidateTokenAsync(this.HttpContext.Request.Headers.Authorization) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}

			GetAllOrdersByUserIdResponse response;
			try
			{
				//Send the query
				response = await this._mediator.Send(new GetAllOrdersByUserIdQuery { UserId = req.UserId }, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to get all Orders");
				await this.SendAsync(new GetAllOrdersByUserIdResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response
			await this.SendOkAsync(response, ct);
		}
	}
}