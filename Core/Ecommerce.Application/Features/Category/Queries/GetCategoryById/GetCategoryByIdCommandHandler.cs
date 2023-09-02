using AutoMapper;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Category;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Features.Category.Queries.GetCategoryById
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="GetCategoryByIdCommand"/>
	/// </summary>
	public class GetCategoryByIdCommandHandler : IRequestHandler<GetCategoryByIdCommand, GetCategoryByIdResponse>
	{
		private readonly ILogger<GetCategoryByIdCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ICategoryAsyncRepository _categoryAsyncRepository;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="GetCategoryByIdCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="categoryAsyncRepository">The <see cref="ICategoryAsyncRepository"/> instance used for data access for <see cref="Category"/> entities.</param>
		public GetCategoryByIdCommandHandler(ILogger<GetCategoryByIdCommandHandler> logger, IMapper mapper, ICategoryAsyncRepository categoryAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._categoryAsyncRepository = categoryAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="GetCategoryByIdCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="GetCategoryByIdCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="GetCategoryByIdResponse"/> with Success being <c>true</c> if the Category was found;
		/// Success will be <c>false</c> if no <see cref="Domain.Entities.Category"/> with the specified ID is found.
		/// Message will contain the error to display if Success is <c>false</c>
		/// </returns>
		public async Task<GetCategoryByIdResponse> Handle(GetCategoryByIdCommand command, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to get an existing category by Id");

			GetCategoryByIdResponse response = new GetCategoryByIdResponse { Success = true, Message = "Successfully Got Category" };

			Domain.Entities.Category? category = await this._categoryAsyncRepository.GetByIdAsync(command.Id);
			response.Category = this._mapper.Map<CategoryDto?>(category);
			
			if (category == null)
			{
				response.Success = false;
				response.Message = "Category was not found";
			}

			return response;
		}
	}
}