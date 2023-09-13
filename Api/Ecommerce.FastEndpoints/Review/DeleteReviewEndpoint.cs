using Ecommerce.Application.Features.Review.Commands.DeleteReview;
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
	/// A Fast Endpoint implementation that handles deleting a Review
	/// </summary>
	public class DeleteReviewEndpoint : Endpoint<DeleteReviewApiRequest, DeleteReviewResponse>
	{
		private readonly ILogger<DeleteReviewEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly IAuthenticationService _authenticationService;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteReviewEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance used for token validation</param>
		public DeleteReviewEndpoint(ILogger<DeleteReviewEndpoint> logger, IMediator mediator, IAuthenticationService authenticationService)
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
			Post("/api/review/delete");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="DeleteReviewApiRequest"/> and generates a <see cref="DeleteReviewResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="DeleteReviewApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(DeleteReviewApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Delete Review Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
			if (await TokenService.ValidateTokenAsync(this._authenticationService, token) == false)
			{
				//Token is Invalid
				await SendUnauthorizedAsync(ct);
				return;
			}

			DeleteReviewResponse response;
			try
			{
				//Send the delete command
				response = await this._mediator.Send(new DeleteReviewCommand { ReviewToDelete = req.ReviewToDelete }, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempt to delete review");
				await SendAsync(new DeleteReviewResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response object
			await SendOkAsync(response, ct);
		}
	}
}