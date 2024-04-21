using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Product.Queries.GetProductsByCategoryId;
using Ecommerce.Shared.Requests.Product;
using Ecommerce.Shared.Responses.Product;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Product
{
	/// <summary>
	/// A Fast Endpoint implementation that handles getting all Products in a Category
	/// </summary>
	public class GetAllProductsByCategoryIdEndpoint : Endpoint<GetAllProductsByCategoryIdApiRequest, GetAllProductsByCategoryIdResponse>
	{
		private readonly ILogger<GetAllProductsByCategoryIdEndpoint> _logger;
		private readonly IMediator _mediator;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetAllProductsByCategoryIdEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		public GetAllProductsByCategoryIdEndpoint(ILogger<GetAllProductsByCategoryIdEndpoint> logger, IMediator mediator)
		{
			this._logger = logger;
			this._mediator = mediator;
		}
		
		/// <summary>
		/// Configures the route for the Endpoint
		/// </summary>
		public override void Configure()
		{
			this.Get("/api/product/all");
			this.AllowAnonymous();
			this.Options(o => o.WithTags("Product"));
		}

		/// <summary>
		/// Handles the <see cref="GetAllProductsByCategoryIdApiRequest"/> and generates a <see cref="GetAllProductsByCategoryIdResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="GetAllProductsByCategoryIdApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(GetAllProductsByCategoryIdApiRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Get All Products by Category Request");
			
			GetAllProductsByCategoryIdResponse response;
			try
			{
				//Send the query
				response = await this._mediator.Send(new GetAllProductsByCategoryIdQuery { CategoryId = req.CategoryId }, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error handling request to get all products by category");
				await this.SendAsync(new GetAllProductsByCategoryIdResponse { Success = false, Message = "Unexpected Error Occurred" }, 500, ct);
				return;
			}

			//Send the response
			await this.SendOkAsync(response, ct);
		}
	}
}