using Ecommerce.Application.Features.Review.Commands.UpdateReview;
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
	public class UpdateReviewEndpoint : Endpoint<UpdateReviewApiRequest, UpdateReviewResponse>
	{
		private readonly ILogger<UpdateReviewEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly IAuthenticationService _authenticationService;

		public UpdateReviewEndpoint(ILogger<UpdateReviewEndpoint> logger, IMediator mediator,
			IAuthenticationService authenticationService)
		{
			this._logger = logger;
			this._mediator = mediator;
			this._authenticationService = authenticationService;
		}
		
		public override void Configure()
		{
			Post("/api/review/update");
		}

		public override async Task HandleAsync(UpdateReviewApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Update Review Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
			if (await TokenService.ValidateTokenAsync(this._authenticationService, token) == false)
			{
				//Token is Invalid
				await SendUnauthorizedAsync(ct);
				return;
			}

			UpdateReviewResponse response;

			try
			{
				response = await this._mediator.Send(new UpdateReviewCommand { ReviewToUpdate = req.ReviewToUpdate }, ct);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, "Error when attempt to update review");
				await SendAsync(new UpdateReviewResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			await SendOkAsync(response, ct);
		}
	}
}