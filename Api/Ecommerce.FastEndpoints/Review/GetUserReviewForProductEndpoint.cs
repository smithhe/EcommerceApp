using Ecommerce.Application.Features.Review.Queries.GetUserReviewForProduct;
using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Requests.Review;
using Ecommerce.Shared.Responses.Review;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Review
{
	/// <summary>
	/// A Fast Endpoint implementation that handles getting a User's Review for a Product
	/// </summary>
	public class GetUserReviewForProductEndpoint : Endpoint<GetUserReviewForProductApiRequest, GetUserReviewForProductResponse>
	{
		private readonly ILogger<GetUserReviewForProductEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly IAuthenticationService _authenticationService;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetUserReviewForProductEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for token validation</param>
		public GetUserReviewForProductEndpoint(ILogger<GetUserReviewForProductEndpoint> logger, IMediator mediator, IAuthenticationService authenticationService)
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
			Get("/api/review/user");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="GetUserReviewForProductApiRequest"/> and generates a <see cref="GetUserReviewForProductResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="GetUserReviewForProductApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(GetUserReviewForProductApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Get User Review Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
			if (await TokenService.ValidateTokenAsync(this._authenticationService, token) == false)
			{
				//Token is Invalid
				await SendUnauthorizedAsync(ct);
				return;
			}

			GetUserReviewForProductResponse response;
			try
			{
				//Send the query
				response = await this._mediator.Send(new GetUserReviewForProductQuery { ProductId = req.ProductId, UserName = req.UserName }, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to get a user review");
				await SendAsync(new GetUserReviewForProductResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response object
			await SendOkAsync(response, ct);
		}
		
	}
}