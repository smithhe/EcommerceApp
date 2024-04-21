using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Product.Commands.UpdateProduct;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.Product;
using Ecommerce.Shared.Responses.Product;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Product
{
	/// <summary>
	/// A Fast Endpoint implementation that handles updating an existing Product
	/// </summary>
	public class UpdateProductEndpoint : Endpoint<UpdateProductApiRequest, UpdateProductResponse>
	{
		private readonly ILogger<UpdateProductEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateProductEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public UpdateProductEndpoint(ILogger<UpdateProductEndpoint> logger, IMediator mediator, ITokenService tokenService)
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
			this.Put("/api/product/update");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="UpdateProductApiRequest"/> and generates a <see cref="UpdateProductResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="UpdateProductApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(UpdateProductApiRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Update Product Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers.Authorization;
			if (await this._tokenService.ValidateTokenAsync(token) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}

			UpdateProductResponse response;
			try
			{
				//Send the update command
				response = await this._mediator.Send(new UpdateProductCommand
				{
					ProductToUpdate = req.ProductToUpdate, 
					UserName = this._tokenService.GetUserNameFromToken(token)
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to update Product");
				await this.SendAsync(new UpdateProductResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response
			await this.SendOkAsync(response, ct);
		}
	}
}