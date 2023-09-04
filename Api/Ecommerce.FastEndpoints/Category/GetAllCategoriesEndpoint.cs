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
	public class GetAllCategoriesEndpoint : Endpoint<GetAllCategoriesApiRequest, GetAllCategoriesResponse>
	{
		private readonly ILogger<GetAllCategoriesEndpoint> _logger;
		private readonly IMediator _mediator;

		public GetAllCategoriesEndpoint(ILogger<GetAllCategoriesEndpoint> logger, IMediator mediator)
		{
			this._logger = logger;
			this._mediator = mediator;
		}
		
		public override void Configure()
		{
			Get("/api/category/all");
			AllowAnonymous();
		}

		public override async Task HandleAsync(GetAllCategoriesApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Get All Categories Request");
			GetAllCategoriesResponse response;
			
			try
			{
				response = await this._mediator.Send(new GetAllCategoriesQuery(), ct);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, "Error handling request to get all categories");
				await SendAsync(new GetAllCategoriesResponse { Success = false, Message = "Unexpected Error Occurred" }, 500, ct);
				return;
			}

			await SendAsync(new GetAllCategoriesResponse { Success = false, Message = "You cant do that" }, 400, ct);
			//await SendOkAsync(response, ct);
		}
	}
}