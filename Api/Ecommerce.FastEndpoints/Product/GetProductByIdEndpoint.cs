using Ecommerce.Application.Features.Product.Queries.GetProductById;
using Ecommerce.Shared.Requests.Product;
using Ecommerce.Shared.Responses.Product;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Product
{
	/// <summary>
	/// A Fast Endpoint implementation that handles getting a Product based on a Id 
	/// </summary>
	public class GetProductByIdEndpoint : Endpoint<GetProductByIdApiRequest, GetProductByIdResponse>
	{
		private readonly ILogger<GetProductByIdEndpoint> _logger;
		private readonly IMediator _mediator;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="GetProductByIdEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		public GetProductByIdEndpoint(ILogger<GetProductByIdEndpoint> logger, IMediator mediator)
		{
			this._logger = logger;
			this._mediator = mediator;
		}
		
		/// <summary>
		/// Configures the route for the Endpoint
		/// </summary>
		public override void Configure()
		{
			Get("/api/product/get");
			AllowAnonymous();
		}

		/// <summary>
		/// Handles the <see cref="GetProductByIdApiRequest"/> and generates a <see cref="GetProductByIdResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="GetProductByIdApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(GetProductByIdApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Get Product By Id Request");
			GetProductByIdResponse response;
			
			try
			{
				//Send the query
				response = await this._mediator.Send(new GetProductByIdQuery { Id = req.ProductId }, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error handling request to get product by id");
				await SendAsync(new GetProductByIdResponse { Success = false, Message = "Unexpected Error Occurred" }, 500, ct);
				return;
			}

			//Send the response object
			await SendOkAsync(response, ct);
		}
	}
}