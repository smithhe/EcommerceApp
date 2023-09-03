using AutoMapper;
using Ecommerce.Application.Validators.Product;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.Product;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Features.Product.Commands.UpdateProduct
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="UpdateProductCommand"/>
	/// </summary>
	public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
	{
		private readonly ILogger<UpdateProductCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IProductAsyncRepository _productAsyncRepository;
		private readonly ICategoryAsyncRepository _categoryAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateProductCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="productAsyncRepository">The <see cref="IProductAsyncRepository"/> instance used for data access for <see cref="Product"/> entities.</param>
		/// <param name="categoryAsyncRepository">The <see cref="ICategoryAsyncRepository"/> instance used for data access for <see cref="Category"/> entities.</param>
		public UpdateProductCommandHandler(ILogger<UpdateProductCommandHandler> logger, IMapper mapper,
			IProductAsyncRepository productAsyncRepository, ICategoryAsyncRepository categoryAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._productAsyncRepository = productAsyncRepository;
			this._categoryAsyncRepository = categoryAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="UpdateProductCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="UpdateProductCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="UpdateProductResponse"/> with Success being <c>true</c> if the <see cref="Product"/> was updated;
		/// Success will be <c>false</c> if no <see cref="Product"/> is found or validation of the command fails.
		/// Message will contain the error to display if Success is <c>false</c>;
		/// Validation Errors will be populated with errors to present if validation fails
		/// </returns>
		public async Task<UpdateProductResponse> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to update an existing product");
			
			UpdateProductResponse response = new UpdateProductResponse { Success = true, Message = "Product Updated Successfully" };
			
			//Check if the dto is null
			if (command.ProductToUpdate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a Product to update";
				return response;
			}
			
			//Validate the dto that was passed in the command
			UpdateProductValidator validator = new UpdateProductValidator(this._productAsyncRepository, this._categoryAsyncRepository);
			ValidationResult? validationResult = await validator.ValidateAsync(command, cancellationToken);
			
			//Check for validation errors
			if (validationResult.Errors.Count > 0)
			{
				this._logger.LogWarning("Command failed validation, returning validation errors");
				
				response.Success = false;
				response.Message = "Command was invalid";
				foreach (ValidationFailure validationResultError in validationResult.Errors)
				{
					response.ValidationErrors.Add(validationResultError.ErrorMessage);
				}

				return response;
			}
			
			//Valid Command
			bool success = await this._productAsyncRepository.UpdateAsync(this._mapper.Map<Domain.Entities.Product>(command.ProductToUpdate));
			
			if (success == false)
			{
				response.Success = false;
				response.Message = "Failed to update the Product";
			}
			
			return response;
		}
	}
}