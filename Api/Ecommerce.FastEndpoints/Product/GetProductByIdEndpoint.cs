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
	public class GetProductByIdEndpoint : Endpoint<GetProductByIdApiRequest, GetProductByIdResponse>
	{
		private readonly ILogger<GetProductByIdEndpoint> _logger;
		private readonly IMediator _mediator;

		public GetProductByIdEndpoint(ILogger<GetProductByIdEndpoint> logger, IMediator mediator)
		{
			this._logger = logger;
			this._mediator = mediator;
		}
		
		public override void Configure()
		{
			Get("/api/product/{ProductId}");
			AllowAnonymous();
		}

		public override async Task HandleAsync(GetProductByIdApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Get Product By Id Request");
			GetProductByIdResponse response;
			
			try
			{
				response = await this._mediator.Send(new GetProductByIdQuery { Id = req.ProductId }, ct);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, "Error handling request to get product by id");
				await SendAsync(new GetProductByIdResponse { Success = false, Message = "Unexpected Error Occurred" }, 500, ct);
				return;
			}

			await SendOkAsync(response, ct);
		}
	}
}