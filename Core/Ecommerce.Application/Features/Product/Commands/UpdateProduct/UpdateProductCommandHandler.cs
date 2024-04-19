using AutoMapper;
using Ecommerce.Application.Validators.Product;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.Product;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.Shared.Extensions;

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
		private readonly IMediator _mediator;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateProductCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="productAsyncRepository">The <see cref="IProductAsyncRepository"/> instance used for data access for <see cref="Product"/> entities.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		public UpdateProductCommandHandler(ILogger<UpdateProductCommandHandler> logger, IMapper mapper,
			IProductAsyncRepository productAsyncRepository, IMediator mediator)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._productAsyncRepository = productAsyncRepository;
			this._mediator = mediator;
		}
		
		/// <summary>
		/// Handles the <see cref="UpdateProductCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="UpdateProductCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="UpdateProductResponse"/> with Success being <c>true</c> if the <see cref="Product"/> was updated;
		/// Success will be <c>false</c> if no <see cref="Product"/> is found or validation of the command fails.
		/// Message will contain the message to display to the user.
		/// Validation Errors will be populated with errors to present if validation fails
		/// </returns>
		public async Task<UpdateProductResponse> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to update an existing product");
			
			//Create the response object
			UpdateProductResponse response = new UpdateProductResponse { Success = true, Message = ProductConstants._updateSuccessMessage };
			
			//Check if the dto is null
			if (command.ProductToUpdate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = ProductConstants._updateErrorMessage;
				return response;
			}
			
			//Check if username is null or empty
			if (string.IsNullOrEmpty(command.UserName))
			{
				this._logger.LogWarning("UserName was null or empty in command, returning failed response");
				response.Success = false;
				response.Message = ProductConstants._updateErrorMessage;
				return response;
			}
			
			//Validate the dto that was passed in the command
			UpdateProductValidator validator = new UpdateProductValidator(this._productAsyncRepository, this._mediator);
			ValidationResult? validationResult = await validator.ValidateAsync(command, cancellationToken);
			
			//Check for validation errors
			if (validationResult.Errors.Count > 0)
			{
				this._logger.LogWarning("Command failed validation, returning validation errors");
				
				response.Success = false;
				response.Message = ProductConstants._genericValidationErrorMessage;
				foreach (ValidationFailure validationResultError in validationResult.Errors)
				{
					response.ValidationErrors.Add(validationResultError.ErrorMessage);
				}

				return response;
			}
			
			//Valid Command
			Domain.Entities.Product productToUpdate = this._mapper.Map<Domain.Entities.Product>(command.ProductToUpdate);
			productToUpdate.LastModifiedBy = command.UserName;
			productToUpdate.LastModifiedDate = DateTime.UtcNow.ToEst();
			
			//Attempt the update
			bool success = await this._productAsyncRepository.UpdateAsync(productToUpdate);
			
			//Check if the update was successful
			if (success == false)
			{
				this._logger.LogError("Sql operation failed, returning failed response");
				
				response.Success = false;
				response.Message = ProductConstants._updateErrorMessage;
			}
			
			//Return the response
			return response;
		}
	}
}