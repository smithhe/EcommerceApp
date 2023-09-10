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
	public class GetUserReviewForProductEndpoint : Endpoint<GetUserReviewForProductApiRequest, GetUserReviewForProductResponse>
	{
		private readonly ILogger<GetUserReviewForProductEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly IAuthenticationService _authenticationService;

		public GetUserReviewForProductEndpoint(ILogger<GetUserReviewForProductEndpoint> logger, IMediator mediator,
			IAuthenticationService authenticationService)
		{
			this._logger = logger;
			this._mediator = mediator;
			this._authenticationService = authenticationService;
		}
		
		public override void Configure()
		{
			Get("/api/review/user");
		}

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
				response = await this._mediator.Send(new GetUserReviewForProductQuery { ProductId = req.ProductId, UserName = req.UserName }, ct);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, "Error when attempt to get a user review");
				await SendAsync(new GetUserReviewForProductResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			await SendOkAsync(response, ct);
		}
		
	}
}