using AutoMapper;
using Ecommerce.Application.Features.Product.Commands.UpdateProduct;
using Ecommerce.Application.Validators.Review;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Review;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Product.Queries.GetProductById;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.Shared.Extensions;

namespace Ecommerce.Application.Features.Review.Commands.CreateReview
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="CreateReviewCommand"/>
	/// </summary>
	public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, CreateReviewResponse>
	{
		private readonly ILogger<CreateReviewCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IMediator _mediator;
		private readonly IReviewAsyncRepository _reviewAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateReviewCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="reviewAsyncRepository">The <see cref="IReviewAsyncRepository"/> instance used for data access for <see cref="Review"/> entities.</param>
		public CreateReviewCommandHandler(ILogger<CreateReviewCommandHandler> logger, IMapper mapper, IMediator mediator,
			IReviewAsyncRepository reviewAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._mediator = mediator;
			this._reviewAsyncRepository = reviewAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="CreateReviewCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="CreateReviewCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="CreateReviewResponse"/> with Success being <c>true</c> if the <see cref="Review"/> was created;
		/// Success will be <c>false</c> if validation of the command fails or Sql fails to create the <see cref="Review"/>.
		/// Message will contain the message to display to the user.
		/// Validation Errors will be populated with errors to present if validation fails.
		/// Review will contain the new <see cref="ReviewDto"/> if creation was successful.
		/// </returns>
		public async Task<CreateReviewResponse> Handle(CreateReviewCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to create a new review");

			//Create the response object
			CreateReviewResponse response = new CreateReviewResponse { Success = true, Message = ReviewConstants._createSuccessMessage };
			
			//Check if the dto is null
			if (command.ReviewToCreate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = ReviewConstants._createErrorMessage;
				return response;
			}
			
			//Check if username is null or empty
			if (string.IsNullOrEmpty(command.UserName))
			{
				this._logger.LogWarning("UserName was null in command, returning failed response");
				response.Success = false;
				response.Message = ReviewConstants._createErrorMessage;
				return response;
			}
			
			//Validate the dto that was passed in the command
			CreateReviewValidator validator = new CreateReviewValidator(this._reviewAsyncRepository, this._mediator);
			ValidationResult validationResult = await validator.ValidateAsync(command, cancellationToken);
			
			//Check for validation errors
			if (validationResult.Errors.Count > 0)
			{
				this._logger.LogWarning("Command failed validation, returning validation errors");
				
				response.Success = false;
				response.Message = ReviewConstants._genericValidationErrorMessage;
				foreach (ValidationFailure validationResultError in validationResult.Errors)
				{
					response.ValidationErrors.Add(validationResultError.ErrorMessage);
				}

				return response;
			}
			
			//Valid Command
			Domain.Entities.Review newReview = this._mapper.Map<Domain.Entities.Review>(command.ReviewToCreate);
			newReview.CreatedBy = command.UserName;
			newReview.CreatedDate = DateTime.UtcNow.ToEst();
			
			//Attempt to add the review
			int newId = await this._reviewAsyncRepository.AddAsync(newReview);
			
			//Sql operation failed, return failed response
			if (newId == -1)
			{
				this._logger.LogError("Sql operation failed, returning failed response");
				
				response.Success = false;
				response.Message = ReviewConstants._createErrorMessage;
				return response;
			}
			
			//Map the return object
			Domain.Entities.Review? review = await this._reviewAsyncRepository.GetByIdAsync(newId);
			response.Review = this._mapper.Map<ReviewDto?>(review);
			
			//Get the average rating for the product
			Domain.Entities.Product? product = this._mapper.Map<Domain.Entities.Product>(
				(await this._mediator.Send(new GetProductByIdQuery { Id = newReview.ProductId }, cancellationToken)).Product);
			decimal newAverageRating = await this._reviewAsyncRepository.GetAverageRatingForProduct(product!.Id);

			//Send the update command
			product.AverageRating = newAverageRating;
			await this._mediator.Send(new UpdateProductCommand
			{
				ProductToUpdate = this._mapper.Map<ProductDto>(product), 
				UserName = "System"
			}, cancellationToken);
			
			//Return the response
			return response;
		}
	}
}