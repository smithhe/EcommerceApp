using System;
using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants;
using Ecommerce.Shared.Enums;

namespace Ecommerce.Application.Features.Order.Queries.GetAllOrdersByUserId
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="GetAllOrdersByUserIdQuery"/>
	/// </summary>
	public class GetAllOrdersByUserIdQueryHandler : IRequestHandler<GetAllOrdersByUserIdQuery, GetAllOrdersByUserIdResponse>
	{
		private readonly ILogger<GetAllOrdersByUserIdQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IOrderAsyncRepository _orderAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetAllOrdersByUserIdQueryHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="orderAsyncRepository">The <see cref="IOrderAsyncRepository"/> instance used for data access for <see cref="Order"/> entities.</param>
		public GetAllOrdersByUserIdQueryHandler(ILogger<GetAllOrdersByUserIdQueryHandler> logger, IMapper mapper, 
			IOrderAsyncRepository orderAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._orderAsyncRepository = orderAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="GetAllOrdersByUserIdQuery"/> request
		/// </summary>
		/// <param name="query">The <see cref="GetAllOrdersByUserIdQuery"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="GetAllOrdersByUserIdResponse"/> with Success being <c>true</c> if any <see cref="Order"/> entities were found;
		/// Success will be <c>false</c> if no <see cref="Order"/> entities were found.
		/// Message will contain the message to display to the user.
		/// Orders will contain all <see cref="Order"/> entities or will be empty if none are found.
		/// </returns>
		public async Task<GetAllOrdersByUserIdResponse> Handle(GetAllOrdersByUserIdQuery query, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to get all existing order entities");

			//Create a new response
			GetAllOrdersByUserIdResponse response = new GetAllOrdersByUserIdResponse { Success = true, Message = OrderConstants._getAllOrdersSuccessMessage };

			//Check if the user id is valid
			if (query.UserId == Guid.Empty)
			{
				response.Success = false;
				response.Message = OrderConstants._getAllOrdersErrorMessage;
				return response;
			}
			
			//Get all orders for the user
			IEnumerable<Domain.Entities.Order> orders = await this._orderAsyncRepository.ListAllAsync(query.UserId);
			
			//Filter out orders in a created or pending status
			orders = orders.Where(o => o.Status != OrderStatus.Created && o.Status != OrderStatus.Pending);

			//No orders found
			if (orders.Any() == false)
			{
				response.Success = false;
				response.Message = OrderConstants._getAllOrdersErrorMessage;
				return response;
			}

			//Map the orders and return the response
			response.Orders = this._mapper.Map<IEnumerable<OrderDto>>(orders);
			return response;
		}
	}
}