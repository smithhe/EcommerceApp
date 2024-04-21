using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Review.Queries.GetUserReviewForProduct;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.Review;
using Ecommerce.Shared.Responses.Review;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Review
{
	/// <summary>
	/// A Fast Endpoint implementation that handles getting a User's Review for a Product
	/// </summary>
	public class GetUserReviewForProductEndpoint : Endpoint<GetUserReviewForProductApiRequest, GetUserReviewForProductResponse>
	{
		private readonly ILogger<GetUserReviewForProductEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetUserReviewForProductEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public GetUserReviewForProductEndpoint(ILogger<GetUserReviewForProductEndpoint> logger, IMediator mediator, ITokenService tokenService)
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
			this.Get("/api/review/user");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="GetUserReviewForProductApiRequest"/> and generates a <see cref="GetUserReviewForProductResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="GetUserReviewForProductApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(GetUserReviewForProductApiRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Get User Review Request");
			
			//Check if token is valid
			if (await this._tokenService.ValidateTokenAsync(this.HttpContext.Request.Headers.Authorization) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
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
				await this.SendAsync(new GetUserReviewForProductResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response
			await this.SendOkAsync(response, ct);
		}
		
	}
}