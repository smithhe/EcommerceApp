using Ecommerce.Application.Features.Product.Queries.GetProductsByCategoryId;
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
	public class GetAllProductsByCategoryIdEndpoint : Endpoint<GetAllProductsByCategoryIdApiRequest, GetAllProductsByCategoryIdResponse>
	{
		private readonly ILogger<GetAllProductsByCategoryIdEndpoint> _logger;
		private readonly IMediator _mediator;

		public GetAllProductsByCategoryIdEndpoint(ILogger<GetAllProductsByCategoryIdEndpoint> logger, IMediator mediator)
		{
			this._logger = logger;
			this._mediator = mediator;
		}
		
		public override void Configure()
		{
			Get("/api/product/all");
			AllowAnonymous();
		}

		public override async Task HandleAsync(GetAllProductsByCategoryIdApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Get All Products by Category Request");
			GetAllProductsByCategoryIdResponse response;
			
			try
			{
				response = await this._mediator.Send(new GetAllProductsByCategoryIdQuery { CategoryId = req.CategoryId }, ct);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, "Error handling request to get all products by category");
				await SendAsync(new GetAllProductsByCategoryIdResponse { Success = false, Message = "Unexpected Error Occurred" }, 500, ct);
				return;
			}

			await SendOkAsync(response, ct);
		}
	}
}