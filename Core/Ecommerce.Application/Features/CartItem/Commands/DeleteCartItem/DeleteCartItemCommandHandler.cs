using AutoMapper;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.CartItem;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants.Entities;

namespace Ecommerce.Application.Features.CartItem.Commands.DeleteCartItem
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="DeleteCartItemCommand"/>
	/// </summary>
	public class DeleteCartItemCommandHandler : IRequestHandler<DeleteCartItemCommand, DeleteCartItemResponse>
	{
		private readonly ILogger<DeleteCartItemCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ICartItemRepository _cartItemRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteCartItemCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="cartItemRepository">The <see cref="ICartItemRepository"/> instance used for data access for <see cref="CartItem"/> entities.</param>
		public DeleteCartItemCommandHandler(ILogger<DeleteCartItemCommandHandler> logger, IMapper mapper, ICartItemRepository cartItemRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._cartItemRepository = cartItemRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="DeleteCartItemCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="DeleteCartItemCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="DeleteCartItemResponse"/> with Success being <c>true</c> if the <see cref="CartItem"/> was deleted;
		/// Success will be <c>false</c> if no <see cref="CartItem"/> is found or validation of the command fails.
		/// Message will contain the message to display to the user.
		/// </returns>
		public async Task<DeleteCartItemResponse> Handle(DeleteCartItemCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to delete a cart item");
			
			//Create the response object
			DeleteCartItemResponse response = new DeleteCartItemResponse { Success = true, Message = CartItemConstants._deleteSingleItemSuccessMessage };
			
			//Check if the dto is null
			if (command.CartItemToDelete == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = CartItemConstants._deleteSingleItemErrorMessage;
				return response;
			}
			
			//Attempt the delete
			bool success = await this._cartItemRepository.DeleteAsync(this._mapper.Map<Domain.Entities.CartItem>(command.CartItemToDelete));
			
			//If the delete failed, update to a failed response
			if (success == false)
			{
				this._logger.LogWarning("Sql returned false, returning failed response");
				
				response.Success = false;
				response.Message = CartItemConstants._deleteSingleItemErrorMessage;
			}
			
			//Return the response
			return response;
		}
	}
}