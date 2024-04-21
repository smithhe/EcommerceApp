using System;
using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.CartItem;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants.Entities;

namespace Ecommerce.Application.Features.CartItem.Queries.GetUserCartItems
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="GetUserCartItemsQuery"/>
	/// </summary>
	public class GetUserCartItemsQueryHandler : IRequestHandler<GetUserCartItemsQuery, GetUserCartItemsResponse>
	{
		private readonly ILogger<GetUserCartItemsQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ICartItemRepository _cartItemRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetUserCartItemsQueryHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="cartItemRepository">The <see cref="ICartItemRepository"/> instance used for data access for <see cref="CartItem"/> entities.</param>
		public GetUserCartItemsQueryHandler(ILogger<GetUserCartItemsQueryHandler> logger, IMapper mapper, ICartItemRepository cartItemRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._cartItemRepository = cartItemRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="GetUserCartItemsQuery"/> request
		/// </summary>
		/// <param name="query">The <see cref="GetUserCartItemsQuery"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="GetUserCartItemsResponse"/> with Success being <c>true</c> if any <see cref="CartItem"/> entities were found;
		/// Success will be <c>false</c> if no <see cref="CartItem"/> entities were found.
		/// Message will contain the message to display to the user.
		/// CartItems will contain all <see cref="CartItemDto"/> entities or will be empty if none are found or an error occurred.
		/// </returns>
		public async Task<GetUserCartItemsResponse> Handle(GetUserCartItemsQuery query, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to get all existing cart item entities for a user");
			
			//Create the response object
			GetUserCartItemsResponse response = new GetUserCartItemsResponse { Success = true, Message = CartItemConstants._getAllItemsSuccessMessage };
			
			//If the user id is empty, return a failed response
			if (query.UserId == Guid.Empty)
			{
				response.Success = false;
				response.Message = CartItemConstants._getAllItemsErrorMessage;
				return response;
			}
			
			//Get all the cart items for the user
			IEnumerable<Domain.Entities.CartItem>? cartItems = await this._cartItemRepository.ListAllAsync(query.UserId);

			//If cart items were null, update to a failed response
			if (cartItems == null)
			{
				response.Success = false;
				response.Message = CartItemConstants._getAllItemsErrorMessage;
			}
			
			//Map the cart items to the response
			response.CartItems = this._mapper.Map<IEnumerable<CartItemDto>>(cartItems);

			//Return the response
			return response;
		}
	}
}