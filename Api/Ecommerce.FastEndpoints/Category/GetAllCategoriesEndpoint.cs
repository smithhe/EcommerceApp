using Ecommerce.Application.Features.Category.Queries.GetAllCategories;
using Ecommerce.Shared.Requests.Category;
using Ecommerce.Shared.Responses.Category;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Category
{
	/// <summary>
	/// A Fast Endpoint implementation that handles getting all Categories
	/// </summary>
	public class GetAllCategoriesEndpoint : Endpoint<GetAllCategoriesApiRequest, GetAllCategoriesResponse>
	{
		private readonly ILogger<GetAllCategoriesEndpoint> _logger;
		private readonly IMediator _mediator;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetAllCategoriesEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		public GetAllCategoriesEndpoint(ILogger<GetAllCategoriesEndpoint> logger, IMediator mediator)
		{
			this._logger = logger;
			this._mediator = mediator;
		}
		
		/// <summary>
		/// Configures the route for the Endpoint
		/// </summary>
		public override void Configure()
		{
			Get("/api/category/all");
			AllowAnonymous();
		}

		/// <summary>
		/// Handles the <see cref="GetAllCategoriesApiRequest"/> and generates a <see cref="GetAllCategoriesResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="GetAllCategoriesApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(GetAllCategoriesApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Get All Categories Request");
			GetAllCategoriesResponse response;
			
			try
			{
				//Send the query
				response = await this._mediator.Send(new GetAllCategoriesQuery(), ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error handling request to get all categories");
				await SendAsync(new GetAllCategoriesResponse { Success = false, Message = "Unexpected Error Occurred" }, 500, ct);
				return;
			}

			//Send the response object
			await SendOkAsync(response, ct);
		}
	}
}