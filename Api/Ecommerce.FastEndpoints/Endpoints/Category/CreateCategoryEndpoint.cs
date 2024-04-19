using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Category.Commands.CreateCategory;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.Category;
using Ecommerce.Shared.Responses.Category;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Category
{
	/// <summary>
	/// A Fast Endpoint implementation that handles creating a new Category
	/// </summary>
	public class CreateCategoryEndpoint : Endpoint<CreateCategoryApiRequest, CreateCategoryResponse>
	{
		private readonly ILogger<CreateCategoryEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateCategoryEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public CreateCategoryEndpoint(ILogger<CreateCategoryEndpoint> logger, IMediator mediator, ITokenService tokenService)
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
			this.Post("/api/category/create");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="CreateCategoryApiRequest"/> and generates a <see cref="CreateCategoryResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="CreateCategoryApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(CreateCategoryApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Create Category Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers.Authorization;
			if (await this._tokenService.ValidateTokenAsync(token) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}

			CreateCategoryResponse response;
			try
			{
				//Send the create command
				response = await this._mediator.Send(new CreateCategoryCommand
				{
					CategoryToCreate = req.CategoryToCreate, 
					UserName = this._tokenService.GetUserNameFromToken(token)
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to create Category");
				await this.SendAsync(new CreateCategoryResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}
			
			//Send the response object
			await this.SendOkAsync(response, ct);
		}
	}
}