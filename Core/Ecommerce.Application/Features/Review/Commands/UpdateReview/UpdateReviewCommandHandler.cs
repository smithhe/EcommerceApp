using AutoMapper;
using Ecommerce.Application.Validators.Review;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.Review;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Features.Review.Commands.UpdateReview
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="UpdateReviewCommand"/>
	/// </summary>
	public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, UpdateReviewResponse>
	{
		private readonly ILogger<UpdateReviewCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IReviewAsyncRepository _reviewAsyncRepository;
		private readonly IProductAsyncRepository _productAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateReviewCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="reviewAsyncRepository">The <see cref="IReviewAsyncRepository"/> instance used for data access for <see cref="Review"/> entities.</param>
		/// <param name="productAsyncRepository">The <see cref="IProductAsyncRepository"/> instance used for data access for <see cref="Product"/> entities.</param>
		public UpdateReviewCommandHandler(ILogger<UpdateReviewCommandHandler> logger, IMapper mapper,
			IReviewAsyncRepository reviewAsyncRepository, IProductAsyncRepository productAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._reviewAsyncRepository = reviewAsyncRepository;
			this._productAsyncRepository = productAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="UpdateReviewCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="UpdateReviewCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="UpdateReviewResponse"/> with Success being <c>true</c> if the <see cref="Review"/> was updated;
		/// Success will be <c>false</c> if no <see cref="Review"/> is found or validation of the command fails;
		/// Message will contain the error to display if Success is <c>false</c>;
		/// Validation Errors will be populated with errors to present if validation fails
		/// </returns>
		public async Task<UpdateReviewResponse> Handle(UpdateReviewCommand command, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to update an existing review");

			UpdateReviewResponse response = new UpdateReviewResponse { Success = true, Message = "Review Updated Successfully" };

			//Check if the dto is null
			if (command.ReviewToUpdate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a Review to update";
				return response;
			}
			
			//Check if username is null or empty
			if (string.IsNullOrEmpty(command.UserName))
			{
				this._logger.LogWarning("UserName was null or empty in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a UserName to update";
				return response;
			}
			
			//Validate the dto that was passed in the command
			UpdateReviewValidator validator = new UpdateReviewValidator(this._reviewAsyncRepository, this._productAsyncRepository);
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

			Domain.Entities.Review? reviewToUpdate = this._mapper.Map<Domain.Entities.Review>(command.ReviewToUpdate);
			reviewToUpdate.LastModifiedBy = command.UserName;
			reviewToUpdate.LastModifiedDate = DateTime.Now;
			
			bool success = await this._reviewAsyncRepository.UpdateAsync(reviewToUpdate);
			
			if (success == false)
			{
				response.Success = false;
				response.Message = "Failed to update the Review";
			}
			
			return response;
		}
	}
}