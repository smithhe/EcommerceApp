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
	public class DeleteReviewEndpoint : Endpoint<DeleteReviewApiRequest, DeleteReviewResponse>
	{
		private readonly ILogger<DeleteReviewEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly IAuthenticationService _authenticationService;

		public DeleteReviewEndpoint(ILogger<DeleteReviewEndpoint> logger, IMediator mediator,
			IAuthenticationService authenticationService)
		{
			this._logger = logger;
			this._mediator = mediator;
			this._authenticationService = authenticationService;
		}
		
		public override void Configure()
		{
			Post("/api/review/delete");
		}

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
				response = await this._mediator.Send(new DeleteReviewCommand { ReviewToDelete = req.ReviewToDelete }, ct);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, "Error when attempt to delete review");
				await SendAsync(new DeleteReviewResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			await SendOkAsync(response, ct);
		}
	}
}