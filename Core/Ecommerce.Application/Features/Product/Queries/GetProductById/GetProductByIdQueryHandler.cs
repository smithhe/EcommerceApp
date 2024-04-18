using AutoMapper;
using Ecommerce.Application.Features.Review.Queries.GetReviewsForProduct;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Product;
using Ecommerce.Shared.Responses.Review;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants;

namespace Ecommerce.Application.Features.Product.Queries.GetProductById
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="GetProductByIdQuery"/>
	/// </summary>
	public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdResponse>
	{
		private readonly ILogger<GetProductByIdQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IMediator _mediator;
		private readonly IProductAsyncRepository _productAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetProductByIdQueryHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="productAsyncRepository">The <see cref="IProductAsyncRepository"/> instance used for data access for <see cref="Product"/> entities.</param>
		public GetProductByIdQueryHandler(ILogger<GetProductByIdQueryHandler> logger, IMapper mapper,
			IMediator mediator, IProductAsyncRepository productAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._mediator = mediator;
			this._productAsyncRepository = productAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="GetProductByIdQuery"/> request
		/// </summary>
		/// <param name="query">The <see cref="GetProductByIdQuery"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="GetProductByIdResponse"/> with Success being <c>true</c> if the <see cref="Product"/> was found;
		/// Success will be <c>false</c> if no <see cref="Product"/> with the specified ID is found.
		/// Message will contain the message to display to the user.
		/// </returns>
		public async Task<GetProductByIdResponse> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to get an existing product by Id");
			
			//Create the response object
			GetProductByIdResponse response = new GetProductByIdResponse { Success = true, Message = ProductConstants._getProductByIdSuccessMessage };

			//Attempt to get the product
			Domain.Entities.Product? product = await this._productAsyncRepository.GetByIdAsync(query.Id);
			response.Product = this._mapper.Map<ProductDto?>(product);
			
			//Check if the product was found
			if (response.Product == null)
			{
				response.Success = false;
				response.Message = ProductConstants._getProductByIdErrorMessage;
				return response;
			}

			//Get the reviews for the product
			GetReviewsForProductResponse reviewsResponse = await this._mediator.Send(new GetReviewsForProductQuery { ProductId = response.Product.Id }, cancellationToken);

			//Check if the reviews were found
			if (reviewsResponse.Success == false)
			{
				this._logger.LogError("Failed to get reviews for product, returning empty reviews array in response");
				
				response.Message = ProductConstants._getProductByIdReviewsNotFoundErrorMessage;
				response.Product.CustomerReviews = Array.Empty<ReviewDto>();
			}
			else
			{
				response.Product.CustomerReviews = reviewsResponse.Reviews;
			}
			
			//Return the response
			return response;
		}
	}
}