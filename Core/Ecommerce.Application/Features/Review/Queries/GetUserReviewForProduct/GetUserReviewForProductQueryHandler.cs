using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Review;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants.Entities;

namespace Ecommerce.Application.Features.Review.Queries.GetUserReviewForProduct
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="GetUserReviewForProductQuery"/>
	/// </summary>
	public class GetUserReviewForProductQueryHandler : IRequestHandler<GetUserReviewForProductQuery, GetUserReviewForProductResponse>
	{
		private readonly ILogger<GetUserReviewForProductQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IReviewAsyncRepository _reviewAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetUserReviewForProductQueryHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="reviewAsyncRepository">The <see cref="IReviewAsyncRepository"/> instance used for data access for <see cref="Review"/> entities.</param>
		public GetUserReviewForProductQueryHandler(ILogger<GetUserReviewForProductQueryHandler> logger, IMapper mapper,
			IReviewAsyncRepository reviewAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._reviewAsyncRepository = reviewAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="GetUserReviewForProductQuery"/> request
		/// </summary>
		/// <param name="query">The <see cref="GetUserReviewForProductQuery"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="GetUserReviewForProductResponse"/> with Success being <c>true</c> if a <see cref="Review"/> is found;
		/// Success will be <c>false</c> if no <see cref="Review"/> is found.
		/// Message will contain the message to display to the user.
		/// UserReview will contain the <see cref="Review"/> of the <see cref="EcommerceUser"/> or will be empty if it does not exist.
		/// </returns>
		public async Task<GetUserReviewForProductResponse> Handle(GetUserReviewForProductQuery query, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to get a user review of a product");

			//Create the response object
			GetUserReviewForProductResponse response = new GetUserReviewForProductResponse { Success = true, Message = ReviewConstants._getUserReviewSuccessMessage };

			//Check for null or empty username
			if (string.IsNullOrEmpty(query.UserName))
			{
				response.Success = false;
				response.Message = ReviewConstants._getUserReviewErrorMessage;
				return response;
			}

			//Get the review from the database
			Domain.Entities.Review? review = await this._reviewAsyncRepository.GetUserReviewForProduct(query.UserName, query.ProductId);

			//No review found
			if (review == null)
			{
				response.Success = false;
				response.Message = ReviewConstants._getUserReviewErrorMessage;
				return response;
			}
			
			//Check if the review has -1 for the ID
			if (review.Id != -1)
			{
				response.UserReview = this._mapper.Map<ReviewDto>(review);
			}

			//Return the response
			return response;
		}
	}
}