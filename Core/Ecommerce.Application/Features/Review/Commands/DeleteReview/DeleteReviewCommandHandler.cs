using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.Review;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Features.Review.Commands.DeleteReview
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="DeleteReviewCommand"/>
	/// </summary>
	public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, DeleteReviewResponse>
	{
		private readonly ILogger<DeleteReviewCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IReviewAsyncRepository _reviewAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteReviewCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="reviewAsyncRepository">The <see cref="IReviewAsyncRepository"/> instance used for data access for <see cref="Review"/> entities.</param>
		public DeleteReviewCommandHandler(ILogger<DeleteReviewCommandHandler> logger, IMapper mapper, IReviewAsyncRepository reviewAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._reviewAsyncRepository = reviewAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="DeleteReviewCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="DeleteReviewCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="DeleteReviewResponse"/> with Success being <c>true</c> if the <see cref="Review"/> was deleted;
		/// Success will be <c>false</c> if no <see cref="Review"/> is found or validation of the command fails.
		/// Message will contain the error to display if Success is <c>false</c>;
		/// Validation Errors will be populated with errors to present if validation fails
		/// </returns>
		public async Task<DeleteReviewResponse> Handle(DeleteReviewCommand command, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to delete a review");

			DeleteReviewResponse response = new DeleteReviewResponse { Success = true, Message = "Review deleted successfully" };

			//Check if the dto is null
			if (command.ReviewToDelete == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a Review to delete";
				return response;
			}
			
			//Attempt the delete
			bool success = await this._reviewAsyncRepository.DeleteAsync(this._mapper.Map<Domain.Entities.Review>(command.ReviewToDelete));

			if (success == false)
			{
				response.Success = false;
				response.Message = "Review failed to delete or doesn't exist";
			}
			
			return response;
		}
	}
}