using AutoMapper;
using Ecommerce.Application.Features.Product.Commands.UpdateProduct;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Review;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Product.Queries.GetProductById;
using Ecommerce.Domain.Constants.Entities;

namespace Ecommerce.Application.Features.Review.Commands.DeleteReview
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="DeleteReviewCommand"/>
	/// </summary>
	public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, DeleteReviewResponse>
	{
		private readonly ILogger<DeleteReviewCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IMediator _mediator;
		private readonly IReviewAsyncRepository _reviewAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteReviewCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="reviewAsyncRepository">The <see cref="IReviewAsyncRepository"/> instance used for data access for <see cref="Review"/> entities.</param>
		public DeleteReviewCommandHandler(ILogger<DeleteReviewCommandHandler> logger, IMapper mapper, IMediator mediator,
			IReviewAsyncRepository reviewAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._mediator = mediator;
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
		/// Message will contain the message to display to the user.
		/// </returns>
		public async Task<DeleteReviewResponse> Handle(DeleteReviewCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to delete a review");

			//Create the response object
			DeleteReviewResponse response = new DeleteReviewResponse { Success = true, Message = ReviewConstants._deleteSuccessMessage };

			//Check if the dto is null
			if (command.ReviewToDelete == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = ReviewConstants._deleteErrorMessage;
				return response;
			}
			
			//Attempt the delete
			bool success = await this._reviewAsyncRepository.DeleteAsync(this._mapper.Map<Domain.Entities.Review>(command.ReviewToDelete));

			if (success == false)
			{
				response.Success = false;
				response.Message = ReviewConstants._deleteErrorMessage;
				return response;
			}
			
			//Get the average rating for the product
			Domain.Entities.Product? product = this._mapper.Map<Domain.Entities.Product>(
				(await this._mediator.Send(new GetProductByIdQuery { Id = command.ReviewToDelete.ProductId }, cancellationToken)).Product);
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