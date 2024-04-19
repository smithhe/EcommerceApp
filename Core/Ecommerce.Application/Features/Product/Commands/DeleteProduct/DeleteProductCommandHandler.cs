using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.Product;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants.Entities;

namespace Ecommerce.Application.Features.Product.Commands.DeleteProduct
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="DeleteProductCommand"/>
	/// </summary>
	public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponse>
	{
		private readonly ILogger<DeleteProductCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IProductAsyncRepository _productAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteProductCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="productAsyncRepository">The <see cref="IProductAsyncRepository"/> instance used for data access for <see cref="Product"/> entities.</param>
		public DeleteProductCommandHandler(ILogger<DeleteProductCommandHandler> logger, IMapper mapper,
			IProductAsyncRepository productAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._productAsyncRepository = productAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="DeleteProductCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="DeleteProductCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="DeleteProductResponse"/> with Success being <c>true</c> if the <see cref="Product"/> was deleted;
		/// Success will be <c>false</c> if no <see cref="Product"/> is found or validation of the command fails.
		/// Message will contain the message to display to the user.
		/// </returns>
		public async Task<DeleteProductResponse> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to delete a product");
			
			//Create the response
			DeleteProductResponse response = new DeleteProductResponse { Success = true, Message = ProductConstants._deleteSuccessMessage };
			
			//Check if the dto is null
			if (command.ProductToDelete == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = ProductConstants._deleteErrorMessage;
				return response;
			}
			
			//Attempt the delete
			bool success = await this._productAsyncRepository.DeleteAsync(this._mapper.Map<Domain.Entities.Product>(command.ProductToDelete));
			
			//Check if the delete was successful
			if (success == false)
			{
				response.Success = false;
				response.Message = ProductConstants._deleteErrorMessage;
			}

			//Return the response
			return response;
		}
	}
}