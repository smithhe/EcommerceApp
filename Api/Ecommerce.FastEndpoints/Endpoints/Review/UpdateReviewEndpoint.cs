using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Review.Commands.UpdateReview;
using Ecommerce.Domain.Constants.Identity;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.Review;
using Ecommerce.Shared.Responses.Review;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Review
{
	/// <summary>
	/// A Fast Endpoint implementation that handles updating an existing Review
	/// </summary>
	public class UpdateReviewEndpoint : Endpoint<UpdateReviewApiRequest, UpdateReviewResponse>
	{
		private readonly ILogger<UpdateReviewEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateReviewEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public UpdateReviewEndpoint(ILogger<UpdateReviewEndpoint> logger, IMediator mediator, ITokenService tokenService)
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
			this.Put("/api/review/update");
			this.Policies(PolicyNames._generalPolicy);
			this.Options(o => o.WithTags("Review"));
		}

		/// <summary>
		/// Handles the <see cref="UpdateReviewApiRequest"/> and generates a <see cref="UpdateReviewResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="UpdateReviewApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(UpdateReviewApiRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Update Review Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers.Authorization;
			if (await this._tokenService.ValidateTokenAsync(token) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}

			UpdateReviewResponse response;
			try
			{
				//Send the update command
				response = await this._mediator.Send(new UpdateReviewCommand
				{
					ReviewToUpdate = req.ReviewToUpdate,
					UserName = this._tokenService.GetUserNameFromToken(token)
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to update review");
				await this.SendAsync(new UpdateReviewResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response
			await this.SendOkAsync(response, ct);
		}
	}
}