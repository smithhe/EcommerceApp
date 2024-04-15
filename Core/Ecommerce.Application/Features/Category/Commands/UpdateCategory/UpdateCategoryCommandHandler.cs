using AutoMapper;
using Ecommerce.Application.Validators.Category;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.Category;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants;

namespace Ecommerce.Application.Features.Category.Commands.UpdateCategory
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="UpdateCategoryCommand"/>
	/// </summary>
	public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponse>
	{
		private readonly ILogger<UpdateCategoryCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ICategoryAsyncRepository _categoryAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateCategoryCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="categoryAsyncRepository">The <see cref="ICategoryAsyncRepository"/> instance used for data access for <see cref="Category"/> entities.</param>
		public UpdateCategoryCommandHandler(ILogger<UpdateCategoryCommandHandler> logger, IMapper mapper, ICategoryAsyncRepository categoryAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._categoryAsyncRepository = categoryAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="UpdateCategoryCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="UpdateCategoryCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="UpdateCategoryResponse"/> with Success being <c>true</c> if the <see cref="Category"/> was updated;
		/// Success will be <c>false</c> if no <see cref="Category"/> is found or validation of the command fails.
		/// Message will contain the message to display to the user.
		/// Validation Errors will be populated with errors to present if validation fails.
		/// </returns>
		public async Task<UpdateCategoryResponse> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to update an existing category");

			//Create the response object
			UpdateCategoryResponse response = new UpdateCategoryResponse { Success = true, Message = CategoryConstants._updateSuccessMessage };
			
			//Check if the dto is null
			if (command.CategoryToUpdate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = CategoryConstants._updateErrorMessage;
				return response;
			}
			
			//Check if username is null or empty
			if (string.IsNullOrEmpty(command.UserName))
			{
				this._logger.LogWarning("UserName was null or empty in command, returning failed response");
				response.Success = false;
				response.Message = CategoryConstants._updateErrorMessage;
				return response;
			}
			
			//Validate the dto that was passed in the command
			UpdateCategoryValidator validator = new UpdateCategoryValidator(this._categoryAsyncRepository);
			ValidationResult? validationResult = await validator.ValidateAsync(command, cancellationToken);

			//Check for validation errors
			if (validationResult.Errors.Count > 0)
			{
				this._logger.LogWarning("Command failed validation, returning validation errors");
				
				response.Success = false;
				response.Message = CategoryConstants._genericValidationErrorMessage;
				foreach (ValidationFailure validationResultError in validationResult.Errors)
				{
					response.ValidationErrors.Add(validationResultError.ErrorMessage);
				}

				return response;
			}
			
			//Valid Command
			Domain.Entities.Category categoryToUpdate = this._mapper.Map<Domain.Entities.Category>(command.CategoryToUpdate);
			categoryToUpdate.LastModifiedBy = command.UserName;
			categoryToUpdate.LastModifiedDate = DateTime.Now;
			
			//Attempt the update
			bool success = await this._categoryAsyncRepository.UpdateAsync(categoryToUpdate);

			//Sql failed to update the category, update the response
			if (success == false)
			{
				response.Success = false;
				response.Message = CategoryConstants._updateErrorMessage;
			}
			
			//Return the response
			return response;
		}
	}
}