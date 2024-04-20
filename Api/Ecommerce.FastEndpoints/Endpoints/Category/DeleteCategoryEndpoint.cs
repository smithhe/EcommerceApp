using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Category.Commands.DeleteCategory;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.Category;
using Ecommerce.Shared.Responses.Category;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Category
{
	/// <summary>
	/// A Fast Endpoint implementation that handles deleting a Category
	/// </summary>
	public class DeleteCategoryEndpoint : Endpoint<DeleteCategoryApiRequest, DeleteCategoryResponse>
	{
		private readonly ILogger<DeleteCategoryEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteCategoryEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public DeleteCategoryEndpoint(ILogger<DeleteCategoryEndpoint> logger, IMediator mediator, ITokenService tokenService)
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
			this.Delete("/api/category/delete");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="DeleteCategoryApiRequest"/> and generates a <see cref="DeleteCategoryResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="DeleteCategoryApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(DeleteCategoryApiRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Delete Category Request");
			
			//Check if token is valid
			if (await this._tokenService.ValidateTokenAsync(this.HttpContext.Request.Headers.Authorization) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}

			DeleteCategoryResponse response;
			try
			{
				//Send the delete command
				response = await this._mediator.Send(new DeleteCategoryCommand
				{
					CategoryToDelete = req.CategoryToDelete, 
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to delete Category");
				await this.SendAsync(new DeleteCategoryResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response
			await this.SendOkAsync(response, ct);
		}
	}
}