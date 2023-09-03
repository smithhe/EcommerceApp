using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Product;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Features.Product.Queries.GetProductById
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="GetProductByIdQuery"/>
	/// </summary>
	public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdResponse>
	{
		private readonly ILogger<GetProductByIdQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IProductAsyncRepository _productAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetProductByIdQueryHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="productAsyncRepository">The <see cref="IProductAsyncRepository"/> instance used for data access for <see cref="Product"/> entities.</param>
		public GetProductByIdQueryHandler(ILogger<GetProductByIdQueryHandler> logger, IMapper mapper,
			IProductAsyncRepository productAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
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
		/// Message will contain the error to display if Success is <c>false</c>
		/// </returns>
		public async Task<GetProductByIdResponse> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to get an existing product by Id");
			
			GetProductByIdResponse response = new GetProductByIdResponse { Success = true, Message = "Successfully Got Product" };

			Domain.Entities.Product? product = await this._productAsyncRepository.GetByIdAsync(query.Id);
			response.Product = this._mapper.Map<ProductDto>(product);
			
			if (product == null)
			{
				response.Success = false;
				response.Message = "Product was not found";
			}

			return response;
		}
	}
}