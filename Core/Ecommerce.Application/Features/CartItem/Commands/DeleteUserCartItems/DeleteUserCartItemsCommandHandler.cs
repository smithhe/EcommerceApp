using System;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.CartItem;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants;

namespace Ecommerce.Application.Features.CartItem.Commands.DeleteUserCartItems
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="DeleteUserCartItemsCommand"/>
	/// </summary>
	public class DeleteUserCartItemsCommandHandler : IRequestHandler<DeleteUserCartItemsCommand, DeleteUserCartItemsResponse>
	{
		private readonly ILogger<DeleteUserCartItemsCommandHandler> _logger;
		private readonly ICartItemRepository _cartItemRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteUserCartItemsCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="cartItemRepository">The <see cref="ICartItemRepository"/> instance used for data access for <see cref="CartItem"/> entities.</param>
		public DeleteUserCartItemsCommandHandler(ILogger<DeleteUserCartItemsCommandHandler> logger, ICartItemRepository cartItemRepository)
		{
			this._logger = logger;
			this._cartItemRepository = cartItemRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="DeleteUserCartItemsCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="DeleteUserCartItemsCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="DeleteUserCartItemsResponse"/> with Success being <c>true</c> if the <see cref="CartItem"/> entities were deleted;
		/// Success will be <c>false</c> if no <see cref="CartItem"/> entities are found or validation of the command fails.
		/// Message will contain the message to display to the user.
		/// </returns>
		public async Task<DeleteUserCartItemsResponse> Handle(DeleteUserCartItemsCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to delete all cart items for a user");
			
			//Create the response object
			DeleteUserCartItemsResponse response = new DeleteUserCartItemsResponse { Success = true, Message = CartItemConstants._deleteAllItemsSuccessMessage };
			
			//Check if the UserId is valid
			if (command.UserId == Guid.Empty)
			{
				response.Success = false;
				response.Message = CartItemConstants._deleteAllItemsErrorMessage;
				return response;
			}
			
			//Attempt the delete
			bool success = await this._cartItemRepository.RemoveUserCartItems(command.UserId);
			
			//If the delete failed, update to a failed response
			if (success == false)
			{
				response.Success = false;
				response.Message = CartItemConstants._deleteAllItemsErrorMessage;
			}
			
			//Return the response
			return response;
		}
	}
}