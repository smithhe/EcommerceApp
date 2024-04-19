using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Review.Commands.CreateReview;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.Review;
using Ecommerce.Shared.Responses.Review;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Review
{
	/// <summary>
	/// A Fast Endpoint implementation that handles creating a new Review
	/// </summary>
	public class CreateReviewEndpoint : Endpoint<CreateReviewApiRequest, CreateReviewResponse>
	{
		private readonly ILogger<CreateReviewEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateReviewEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public CreateReviewEndpoint(ILogger<CreateReviewEndpoint> logger, IMediator mediator, ITokenService tokenService)
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
			this.Post("/api/review/create");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="CreateReviewApiRequest"/> and generates a <see cref="CreateReviewResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="CreateReviewApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(CreateReviewApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Create Review Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers.Authorization;
			if (await this._tokenService.ValidateTokenAsync(token) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}

			CreateReviewResponse response;
			try
			{
				//Send the create command
				response = await this._mediator.Send(new CreateReviewCommand
				{
					ReviewToCreate = req.ReviewToCreate, 
					UserName = this._tokenService.GetUserNameFromToken(token)
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to create review");
				await this.SendAsync(new CreateReviewResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response object
			await this.SendOkAsync(response, ct);
		}
	}
}