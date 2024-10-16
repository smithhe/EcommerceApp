using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.Category;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants.Entities;

namespace Ecommerce.Application.Features.Category.Commands.DeleteCategory
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="DeleteCategoryCommand"/>
	/// </summary>
	public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, DeleteCategoryResponse>
	{
		private readonly ILogger<DeleteCategoryCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ICategoryAsyncRepository _categoryAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteCategoryCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="categoryAsyncRepository">The <see cref="ICategoryAsyncRepository"/> instance used for data access for <see cref="Category"/> entities.</param>
		public DeleteCategoryCommandHandler(ILogger<DeleteCategoryCommandHandler> logger, IMapper mapper, ICategoryAsyncRepository categoryAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._categoryAsyncRepository = categoryAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="DeleteCategoryCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="DeleteCategoryCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="DeleteCategoryResponse"/> with Success being <c>true</c> if the <see cref="Category"/> was deleted;
		/// Success will be <c>false</c> if no <see cref="Category"/> is found or validation of the command fails.
		/// Message will contain the message to display to the user.
		/// </returns>
		public async Task<DeleteCategoryResponse> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to delete a category");

			//Create the response object
			DeleteCategoryResponse response = new DeleteCategoryResponse { Success = true, Message = CategoryConstants._deleteSuccessMessage };

			//Check if the dto is null
			if (command.CategoryToDelete == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = CategoryConstants._deleteErrorMessage;
				return response;
			}
			
			//Attempt the delete
			bool success = await this._categoryAsyncRepository.DeleteAsync(this._mapper.Map<Domain.Entities.Category>(command.CategoryToDelete));

			//Sql failed to delete the category, update the response
			if (success == false)
			{
				response.Success = false;
				response.Message = CategoryConstants._deleteErrorMessage;
			}

			//Return the response
			return response;
		}
	}
}