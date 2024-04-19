using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Product.Commands.DeleteProduct;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Requests.Product;
using Ecommerce.Shared.Responses.Product;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Product
{
	/// <summary>
	/// A Fast Endpoint implementation that handles deleting a Product
	/// </summary>
	public class DeleteProductEndpoint : Endpoint<DeleteProductApiRequest, DeleteProductResponse>
	{
		private readonly ILogger<DeleteProductEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteProductEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		public DeleteProductEndpoint(ILogger<DeleteProductEndpoint> logger, IMediator mediator, ITokenService tokenService)
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
			this.Delete("/api/product/delete");
			//TODO: Add roles
		}

		/// <summary>
		/// Handles the <see cref="DeleteProductApiRequest"/> and generates a <see cref="DeleteProductResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="DeleteProductApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(DeleteProductApiRequest req, CancellationToken ct)
		{
			this._logger.LogInformation("Handling Delete Product Request");
			
			//Check if token is valid
			if (await this._tokenService.ValidateTokenAsync(this.HttpContext.Request.Headers.Authorization) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}

			DeleteProductResponse response;
			try
			{
				//Send the delete command
				response = await this._mediator.Send(new DeleteProductCommand
				{
					ProductToDelete = req.ProductToDelete, 
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to delete Product");
				await this.SendAsync(new DeleteProductResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}

			//Send the response object
			await this.SendOkAsync(response, ct);
		}
	}
}