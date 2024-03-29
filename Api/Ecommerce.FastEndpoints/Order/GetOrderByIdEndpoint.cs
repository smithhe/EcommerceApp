using Ecommerce.Application.Features.Order.Queries.GetOrderById;
using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Requests.Order;
using Ecommerce.Shared.Responses.Order;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Order
{
	/// <summary>
	/// A Fast Endpoint implementation that handles getting a Order based on a Id 
	/// </summary>
	public class GetOrderByIdEndpoint : Endpoint<GetOrderByIdApiRequest, GetOrderByIdResponse>
	{
		private readonly ILogger<GetOrderByIdEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly IAuthenticationService _authenticationService;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetOrderByIdEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for token validation</param>
		public GetOrderByIdEndpoint(ILogger<GetOrderByIdEndpoint> logger, IMediator mediator, IAuthenticationService authenticationService)
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
			Get("/api/order");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="GetOrderByIdApiRequest"/> and generates a <see cref="GetOrderByIdResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="GetOrderByIdApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(GetOrderByIdApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Get Order Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
			if (await TokenService.ValidateTokenAsync(this._authenticationService, token) == false)
			{
				//Token is Invalid
				await SendUnauthorizedAsync(ct);
				return;
			}

			GetOrderByIdResponse response;
			try
			{
				//Send the query
				response = await this._mediator.Send(new GetOrderByIdQuery { Id = req.Id }, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to get a Order");
				await SendAsync(new GetOrderByIdResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response object
			await SendOkAsync(response, ct);
		}
	}
}