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
using Ecommerce.Domain.Constants;
using Ecommerce.Shared.Extensions;

namespace Ecommerce.Application.Features.Review.Commands.UpdateReview
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="UpdateReviewCommand"/>
	/// </summary>
	public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, UpdateReviewResponse>
	{
		private readonly ILogger<UpdateReviewCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IMediator _mediator;
		private readonly IReviewAsyncRepository _reviewAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateReviewCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="reviewAsyncRepository">The <see cref="IReviewAsyncRepository"/> instance used for data access for <see cref="Review"/> entities.</param>
		public UpdateReviewCommandHandler(ILogger<UpdateReviewCommandHandler> logger, IMapper mapper, IMediator mediator,
			IReviewAsyncRepository reviewAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._mediator = mediator;
			this._reviewAsyncRepository = reviewAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="UpdateReviewCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="UpdateReviewCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="UpdateReviewResponse"/> with Success being <c>true</c> if the <see cref="Review"/> was updated;
		/// Success will be <c>false</c> if no <see cref="Review"/> is found or validation of the command fails.
		/// Message will contain the message to display to the user..
		/// Validation Errors will be populated with errors to present if validation fails.
		/// </returns>
		public async Task<UpdateReviewResponse> Handle(UpdateReviewCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to update an existing review");

			//Create the response object
			UpdateReviewResponse response = new UpdateReviewResponse { Success = true, Message = ReviewConstants._updateSuccessMessage };

			//Check if the dto is null
			if (command.ReviewToUpdate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = ReviewConstants._updateErrorMessage;
				return response;
			}
			
			//Check if username is null or empty
			if (string.IsNullOrEmpty(command.UserName))
			{
				this._logger.LogWarning("UserName was null or empty in command, returning failed response");
				response.Success = false;
				response.Message = ReviewConstants._updateErrorMessage;
				return response;
			}
			
			//Validate the dto that was passed in the command
			UpdateReviewValidator validator = new UpdateReviewValidator(this._reviewAsyncRepository, this._mediator);
			ValidationResult? validationResult = await validator.ValidateAsync(command, cancellationToken);

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

			//Update the review
			Domain.Entities.Review? reviewToUpdate = this._mapper.Map<Domain.Entities.Review>(command.ReviewToUpdate);
			reviewToUpdate.LastModifiedBy = command.UserName;
			reviewToUpdate.LastModifiedDate = DateTime.UtcNow.ToEst();
			
			//Attempt the update
			bool success = await this._reviewAsyncRepository.UpdateAsync(reviewToUpdate);
			
			//Check if the update failed
			if (success == false)
			{
				response.Success = false;
				response.Message = ReviewConstants._updateErrorMessage;
				return response;
			}
			
			//Get the average rating for the product
			Domain.Entities.Product? product = this._mapper.Map<Domain.Entities.Product>(
				(await this._mediator.Send(new GetProductByIdQuery { Id = reviewToUpdate.ProductId }, cancellationToken)).Product);
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