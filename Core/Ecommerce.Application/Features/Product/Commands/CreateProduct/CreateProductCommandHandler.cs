using AutoMapper;
using Ecommerce.Application.Validators.Product;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Product;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Features.Product.Commands.CreateProduct
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="CreateProductCommand"/>
	/// </summary>
	public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
	{
		private readonly ILogger<CreateProductCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IProductAsyncRepository _productAsyncRepository;
		private readonly ICategoryAsyncRepository _categoryAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateProductCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="productAsyncRepository">The <see cref="IProductAsyncRepository"/> instance used for data access for <see cref="Product"/> entities.</param>
		/// <param name="categoryAsyncRepository">The <see cref="ICategoryAsyncRepository"/> instance used for data access for <see cref="Category"/> entities.</param>
		public CreateProductCommandHandler(ILogger<CreateProductCommandHandler> logger, IMapper mapper,
			IProductAsyncRepository productAsyncRepository, ICategoryAsyncRepository categoryAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._productAsyncRepository = productAsyncRepository;
			this._categoryAsyncRepository = categoryAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="CreateProductCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="CreateProductCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="CreateProductResponse"/> with Success being <c>true</c> if the <see cref="Product"/> was created;
		/// Success will be <c>false</c> if validation of the command fails or Sql fails to create the <see cref="Product"/>.
		/// Message will contain the error to display if Success is <c>false</c>.
		/// Validation Errors will be populated with errors to present if validation fails
		/// Product will contain the new <see cref="ProductDto"/> if creation was successful
		/// </returns>
		public async Task<CreateProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to create a new product");

			CreateProductResponse response = new CreateProductResponse { Success = true, Message = "Successfully Created Product" };
			
			//Check if the dto is null
			if (command.ProductToCreate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a Product to create";
				return response;
			}
			
			//Validate the dto that was passed in the command
			CreateProductValidator validator = new CreateProductValidator(this._productAsyncRepository, this._categoryAsyncRepository);
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
			int newId = await this._productAsyncRepository.AddAsync(this._mapper.Map<Domain.Entities.Product>(command.ProductToCreate));
			
			//Sql operation failed
			if (newId == -1)
			{
				response.Success = false;
				response.Message = "Failed to add new Product";
			}
			else
			{
				Domain.Entities.Product? product = await this._productAsyncRepository.GetByIdAsync(newId);
				response.Product = this._mapper.Map<ProductDto>(product);
			}
			
			return response;
		}
	}
}