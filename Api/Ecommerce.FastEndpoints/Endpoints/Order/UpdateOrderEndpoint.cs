using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Order.Commands.UpdateOrder;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.Order;
using Ecommerce.Shared.Responses.Order;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Order
{
	/// <summary>
	/// A Fast Endpoint implementation that handles updating an existing Order
	/// </summary>
	public class UpdateOrderEndpoint : Endpoint<UpdateOrderApiRequest, UpdateOrderResponse>
	{
		private readonly ILogger<UpdateOrderEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateOrderEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public UpdateOrderEndpoint(ILogger<UpdateOrderEndpoint> logger, IMediator mediator, ITokenService tokenService)
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
			this.Put("/api/order/update");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="UpdateOrderApiRequest"/> and generates a <see cref="UpdateOrderResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="UpdateOrderApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(UpdateOrderApiRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Update Order Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers.Authorization;
			if (await this._tokenService.ValidateTokenAsync(token) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}

			UpdateOrderResponse response;
			try
			{
				//Send the update command
				response = await this._mediator.Send(new UpdateOrderCommand
				{
					OrderToUpdate = req.OrderToUpdate, 
					UserName = this._tokenService.GetUserNameFromToken(token)
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to update Order");
				await this.SendAsync(new UpdateOrderResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response
			await this.SendOkAsync(response, ct);
		}
	}
}