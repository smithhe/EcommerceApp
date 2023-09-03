using AutoMapper;
using Ecommerce.Application.Features.OrderItem.Queries.GetAllOrderItemsByOrderId;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;
using Ecommerce.Shared.Responses.OrderItem;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Features.Order.Queries.GetOrderById
{
	public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, GetOrderByIdResponse>
	{
		private readonly ILogger<GetOrderByIdQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IMediator _mediator;
		private readonly IOrderAsyncRepository _orderAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetOrderByIdQueryHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="orderAsyncRepository">The <see cref="IOrderAsyncRepository"/> instance used for data access for <see cref="Order"/> entities.</param>
		public GetOrderByIdQueryHandler(ILogger<GetOrderByIdQueryHandler> logger, IMapper mapper, IMediator mediator,
			IOrderAsyncRepository orderAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._mediator = mediator;
			this._orderAsyncRepository = orderAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="GetOrderByIdQuery"/> request
		/// </summary>
		/// <param name="query">The <see cref="GetOrderByIdQuery"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="GetOrderByIdResponse"/> with Success being <c>true</c> if the <see cref="Order"/> was found;
		/// Success will be <c>false</c> if no <see cref="Order"/> with the specified ID is found.
		/// Message will contain the error to display if Success is <c>false</c>
		/// </returns>
		public async Task<GetOrderByIdResponse> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to get an existing Order by Id");

			GetOrderByIdResponse response = new GetOrderByIdResponse { Success = true, Message = "Successfully Got Order" };

			Domain.Entities.Order? order = await this._orderAsyncRepository.GetByIdAsync(query.Id);

			//Check if the order was found or not
			if (order == null)
			{
				response.Success = false;
				response.Message = "Order does not exist";
				return response;
			}
			
			GetAllOrderItemsByOrderIdResponse orderItemsResponse = await this._mediator.Send(new GetAllOrderItemsByOrderIdQuery { OrderId = query.Id }, cancellationToken);

			//Check if the order items were retrieved successfully
			if (response.Success == false)
			{
				response.Success = false;
				response.Message = "Failed to get order items";
				return response;
			}
			
			response.Order = this._mapper.Map<OrderDto>(order);
			response.Order.OrderItems = orderItemsResponse.OrderItems;
			
			return response;
		}
	}
}