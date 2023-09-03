using AutoMapper;
using Ecommerce.Application.Validators.Category;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Category;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Features.Category.Commands.CreateCategory
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="CreateCategoryCommand"/>
	/// </summary>
	public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
	{
		private readonly ILogger<CreateCategoryCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ICategoryAsyncRepository _categoryAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateCategoryCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="categoryAsyncRepository">The <see cref="ICategoryAsyncRepository"/> instance used for data access for <see cref="Category"/> entities.</param>
		public CreateCategoryCommandHandler(ILogger<CreateCategoryCommandHandler> logger, IMapper mapper, ICategoryAsyncRepository categoryAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._categoryAsyncRepository = categoryAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="CreateCategoryCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="CreateCategoryCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="CreateCategoryResponse"/> with Success being <c>true</c> if the <see cref="Category"/> was created;
		/// Success will be <c>false</c> if validation of the command fails or Sql fails to create the <see cref="Category"/>.
		/// Message will contain the error to display if Success is <c>false</c>;
		/// Validation Errors will be populated with errors to present if validation fails
		/// </returns>
		public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to create a new category");

			CreateCategoryResponse response = new CreateCategoryResponse { Success = true, Message = "Successfully Created Category" };
			
			//Check if the dto is null
			if (command.CategoryToCreate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a Category to create";
				return response;
			}

			//Validate the dto that was passed in the command
			CreateCategoryValidator validator = new CreateCategoryValidator(this._categoryAsyncRepository);
			ValidationResult validationResult = await validator.ValidateAsync(command, cancellationToken);

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
			//TODO: Add user who created the order
			int newId = await this._categoryAsyncRepository.AddAsync(this._mapper.Map<Domain.Entities.Category>(command.CategoryToCreate));

			//Sql operation failed
			if (newId == -1)
			{
				response.Success = false;
				response.Message = "Failed to add new Category";
			}
			else
			{
				Domain.Entities.Category? category = await this._categoryAsyncRepository.GetByIdAsync(newId);
				response.Category = this._mapper.Map<CategoryDto>(category);
			}
			
			return response;
		}
	}
}