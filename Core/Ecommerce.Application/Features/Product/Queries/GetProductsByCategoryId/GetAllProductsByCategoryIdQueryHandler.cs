using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Product;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants.Entities;

namespace Ecommerce.Application.Features.Product.Queries.GetProductsByCategoryId
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="GetAllProductsByCategoryIdQuery"/>
	/// </summary>
	public class GetAllProductsByCategoryIdQueryHandler : IRequestHandler<GetAllProductsByCategoryIdQuery, GetAllProductsByCategoryIdResponse>
	{
		private readonly ILogger<GetAllProductsByCategoryIdQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IProductAsyncRepository _productAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetAllProductsByCategoryIdQueryHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="productAsyncRepository">The <see cref="IProductAsyncRepository"/> instance used for data access for <see cref="Product"/> entities.</param>
		public GetAllProductsByCategoryIdQueryHandler(ILogger<GetAllProductsByCategoryIdQueryHandler> logger, IMapper mapper,
			IProductAsyncRepository productAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._productAsyncRepository = productAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="GetAllProductsByCategoryIdQuery"/> request
		/// </summary>
		/// <param name="query">The <see cref="GetAllProductsByCategoryIdQuery"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="GetAllProductsByCategoryIdResponse"/> with Success being <c>true</c> if any <see cref="Product"/> entities were found;
		/// Success will be <c>false</c> if no <see cref="Product"/> entities were found.
		/// Message will contain the message to display to the user.
		/// Products will contain all <see cref="Product"/> entities or will be empty if none are found
		/// </returns>
		public async Task<GetAllProductsByCategoryIdResponse> Handle(GetAllProductsByCategoryIdQuery query, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to get all existing product entities for a category");
			
			//Create the response object
			GetAllProductsByCategoryIdResponse response = new GetAllProductsByCategoryIdResponse { Success = true, Message = ProductConstants._getAllProductsByCategorySuccessMessage };

			//Get all products for the category
			IEnumerable<Domain.Entities.Product> products = await this._productAsyncRepository.ListAllAsync(query.CategoryId);
			response.Products = this._mapper.Map<IEnumerable<ProductDto>>(products);
			
			//Check if any products were found
			if (products.Any() == false)
			{
				this._logger.LogWarning("Failed to find any products for the category");
				
				response.Success = false;
				response.Message = ProductConstants._getAllProductsByCategoryErrorMessage;
			}

			//Return the response
			return response;
		}
	}
}