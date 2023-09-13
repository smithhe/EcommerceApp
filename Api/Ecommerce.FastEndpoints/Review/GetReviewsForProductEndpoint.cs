using Ecommerce.Application.Features.Review.Queries.GetReviewsForProduct;
using Ecommerce.Shared.Requests.Review;
using Ecommerce.Shared.Responses.Review;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Review
{
	/// <summary>
	/// A Fast Endpoint implementation that handles getting all Reviews for a Product
	/// </summary>
	public class GetReviewsForProductEndpoint : Endpoint<GetReviewsForProductApiRequest, GetReviewsForProductResponse>
	{
		private readonly ILogger<GetReviewsForProductEndpoint> _logger;
		private readonly IMediator _mediator;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetUserReviewForProductEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		public GetReviewsForProductEndpoint(ILogger<GetReviewsForProductEndpoint> logger, IMediator mediator)
		{
			this._logger = logger;
			this._mediator = mediator;
		}
		
		/// <summary>
		/// Configures the route for the Endpoint
		/// </summary>
		public override void Configure()
		{
			Get("/api/review/all");
			AllowAnonymous();
		}

		/// <summary>
		/// Handles the <see cref="GetReviewsForProductApiRequest"/> and generates a <see cref="GetReviewsForProductResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="GetReviewsForProductApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(GetReviewsForProductApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Get All Reviews Request");
			GetReviewsForProductResponse response;

			try
			{
				//Send the query
				response = await this._mediator.Send(new GetReviewsForProductQuery { ProductId = req.ProductId }, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to get all reviews for product");
				await SendAsync(new GetReviewsForProductResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response object
			await SendOkAsync(response, ct);
		}
	}
}