using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Review;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants;

namespace Ecommerce.Application.Features.Review.Queries.GetReviewsForProduct
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="GetReviewsForProductQuery"/>
	/// </summary>
	public class GetReviewsForProductQueryHandler : IRequestHandler<GetReviewsForProductQuery, GetReviewsForProductResponse>
	{
		private readonly ILogger<GetReviewsForProductQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IReviewAsyncRepository _reviewAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetReviewsForProductQueryHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="reviewAsyncRepository">The <see cref="IReviewAsyncRepository"/> instance used for data access for <see cref="Review"/> entities.</param>
		public GetReviewsForProductQueryHandler(ILogger<GetReviewsForProductQueryHandler> logger, IMapper mapper,
			IReviewAsyncRepository reviewAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._reviewAsyncRepository = reviewAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="GetReviewsForProductQuery"/> request
		/// </summary>
		/// <param name="query">The <see cref="GetReviewsForProductQuery"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="GetReviewsForProductResponse"/> with Success being <c>true</c> if any <see cref="Review"/> entities were found;
		/// Success will be <c>false</c> if no <see cref="Review"/> entities were found.
		/// Message will contain the message to display to the user.
		/// Reviews will contain all <see cref="Review"/> entities or will be empty if none are found.
		/// </returns>
		public async Task<GetReviewsForProductResponse> Handle(GetReviewsForProductQuery query, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to get all existing review entities for a product");
			
			//Create the response object
			GetReviewsForProductResponse response = new GetReviewsForProductResponse { Success = true, Message = ReviewConstants._getAllSuccessMessage };

			//Get all reviews for the product
			IEnumerable<Domain.Entities.Review> reviews = await this._reviewAsyncRepository.ListAllAsync(query.ProductId);
			response.Reviews = this._mapper.Map<IEnumerable<ReviewDto>>(reviews);

			//Check if any reviews were found
			if (reviews.Any() == false)
			{
				response.Success = false;
				response.Message = ReviewConstants._getAllErrorMessage;
			}

			//Return the response
			return response;
		}
	}
}