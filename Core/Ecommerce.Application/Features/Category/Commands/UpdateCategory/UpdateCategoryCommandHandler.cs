using AutoMapper;
using Ecommerce.Application.Validators.Category;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.Category;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

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
		public async Task<UpdateCategoryResponse> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to update an existing category");

			UpdateCategoryResponse response = new UpdateCategoryResponse { Success = true, Message = "Successfully updated Category" };
			
			//Check if the dto is null
			if (command.CategoryToUpdate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a Category to update";
				return response;
			}
			
			//Validate the dto that was passed in the command
			UpdateCategoryValidator validator = new UpdateCategoryValidator(this._categoryAsyncRepository);
			ValidationResult? validationResult = await validator.ValidateAsync(command, cancellationToken);

			//Check for validation errors
			if (validationResult.Errors.Count > 0)
			{
				this._logger.LogWarning("Dto failed validation, returning validation errors");
				
				response.Success = false;
				response.Message = "Category was invalid";
				foreach (ValidationFailure validationResultError in validationResult.Errors)
				{
					response.ValidationErrors.Add(validationResultError.ErrorMessage);
				}

				return response;
			}
			
			//Valid Category
			bool success = await this._categoryAsyncRepository.UpdateAsync(this._mapper.Map<Domain.Entities.Category>(command.CategoryToUpdate));

			if (success == false)
			{
				response.Success = false;
				response.Message = "Failed to update the Category";
			}
			
			return response;
		}
	}
}