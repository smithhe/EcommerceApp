using AutoMapper;
using Ecommerce.Application.Validators.Product;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Product;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants;
using Ecommerce.Shared.Extensions;

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
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProductCommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
        /// <param name="productAsyncRepository">The <see cref="IProductAsyncRepository"/> instance used for data access for <see cref="Product"/> entities.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        public CreateProductCommandHandler(ILogger<CreateProductCommandHandler> logger, IMapper mapper,
            IProductAsyncRepository productAsyncRepository, IMediator mediator)
        {
            this._logger = logger;
            this._mapper = mapper;
            this._productAsyncRepository = productAsyncRepository;
            this._mediator = mediator;
        }

        /// <summary>
        /// Handles the <see cref="CreateProductCommand"/> request
        /// </summary>
        /// <param name="command">The <see cref="CreateProductCommand"/> request to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// A <see cref="CreateProductResponse"/> with Success being <c>true</c> if the <see cref="Product"/> was created;
        /// Success will be <c>false</c> if validation of the command fails or Sql fails to create the <see cref="Product"/>.
        /// Message will contain the message to display to the user.
        /// Validation Errors will be populated with errors to present if validation fails.
        /// Product will contain the new <see cref="ProductDto"/> if creation was successful.
        /// </returns>
        public async Task<CreateProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            //Log the request
            this._logger.LogInformation("Handling request to create a new product");

            //Create the response object
            CreateProductResponse response = new CreateProductResponse { Success = true, Message = ProductConstants._createSuccessMessage };

            //Check if the dto is null
            if (command.ProductToCreate == null)
            {
                this._logger.LogWarning("Dto was null in command, returning failed response");
                response.Success = false;
                response.Message = ProductConstants._createErrorMessage;
                return response;
            }

            //Check if username is null or empty
            if (string.IsNullOrEmpty(command.UserName))
            {
                this._logger.LogWarning("UserName was null or empty in command, returning failed response");
                response.Success = false;
                response.Message = ProductConstants._createErrorMessage;
                return response;
            }

            //Validate the dto that was passed in the command
            CreateProductValidator validator = new CreateProductValidator(this._productAsyncRepository, this._mediator);
            ValidationResult validationResult = await validator.ValidateAsync(command, cancellationToken);

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
            Domain.Entities.Product productToCreate = this._mapper.Map<Domain.Entities.Product>(command.ProductToCreate);
            productToCreate.CreatedBy = command.UserName;
            productToCreate.CreatedDate = DateTime.UtcNow.ToEst();

            //Add the product
            int newId = await this._productAsyncRepository.AddAsync(productToCreate);

            //Sql operation failed, return failed response
            if (newId == -1)
            {
                this._logger.LogError("Sql operation failed, returning failed response");
                
                response.Success = false;
                response.Message = ProductConstants._createErrorMessage;
                return response;
            }

            //Get the new product
            Domain.Entities.Product? product = await this._productAsyncRepository.GetByIdAsync(newId);
            response.Product = this._mapper.Map<ProductDto>(product);

            //Return the response
            return response;
        }
    }
}