using Ecommerce.Application.Features.Review.Commands.CreateReview;
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
	public class CreateReviewEndpoint : Endpoint<CreateReviewApiRequest, CreateReviewResponse>
	{
		private readonly ILogger<CreateReviewEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly IAuthenticationService _authenticationService;

		public CreateReviewEndpoint(ILogger<CreateReviewEndpoint> logger, IMediator mediator,
			IAuthenticationService authenticationService)
		{
			this._logger = logger;
			this._mediator = mediator;
			this._authenticationService = authenticationService;
		}
		
		public override void Configure()
		{
			Post("/api/review/create");
			AllowAnonymous();
		}

		public override async Task HandleAsync(CreateReviewApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Create Review Request");
			
			string? token = this.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
			
			if (await TokenService.ValidateTokenAsync(this._authenticationService, token) == false)
			{
				//Token is Invalid
				await SendUnauthorizedAsync(ct);
				return;
			}

			CreateReviewResponse response;

			try
			{
				response = await this._mediator.Send(new CreateReviewCommand
				{
					ReviewToCreate = req.ReviewToCreate, 
					UserName = TokenService.GetUserNameFromToken(token)
				}, ct);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, "Error when attempt to create review");
				await SendAsync(new CreateReviewResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			await SendOkAsync(response, ct);
		}
	}
}