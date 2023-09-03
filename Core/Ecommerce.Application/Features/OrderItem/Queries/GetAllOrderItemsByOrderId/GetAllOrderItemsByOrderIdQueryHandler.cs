using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.OrderItem;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Features.OrderItem.Queries.GetAllOrderItemsByOrderId
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="GetAllOrderItemsByOrderIdQuery"/>
	/// </summary>
	public class GetAllOrderItemsByOrderIdQueryHandler : IRequestHandler<GetAllOrderItemsByOrderIdQuery, GetAllOrderItemsByOrderIdResponse>
	{
		private readonly ILogger<GetAllOrderItemsByOrderIdQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IOrderItemAsyncRepository _orderItemAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetAllOrderItemsByOrderIdQueryHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="orderItemAsyncRepository">The <see cref="IOrderItemAsyncRepository"/> instance used for data access for <see cref="OrderItem"/> entities.</param>
		public GetAllOrderItemsByOrderIdQueryHandler(ILogger<GetAllOrderItemsByOrderIdQueryHandler> logger, IMapper mapper, 
			IOrderItemAsyncRepository orderItemAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._orderItemAsyncRepository = orderItemAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="GetAllOrderItemsByOrderIdQuery"/> request
		/// </summary>
		/// <param name="query">The <see cref="GetAllOrderItemsByOrderIdQuery"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="GetAllOrderItemsByOrderIdResponse"/> with Success being <c>true</c> if any <see cref="OrderItem"/> entities were found;
		/// Success will be <c>false</c> if no <see cref="OrderItem"/> entities were found.
		/// Message will contain the error to display if Success is <c>false</c>
		/// OrderItems will contain all <see cref="OrderItem"/> entities or will be empty if none are found
		/// </returns>
		public async Task<GetAllOrderItemsByOrderIdResponse> Handle(GetAllOrderItemsByOrderIdQuery query, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to get all existing order item entities");
			
			GetAllOrderItemsByOrderIdResponse response = new GetAllOrderItemsByOrderIdResponse { Success = true, Message = "Successfully Got all OrderItems" };

			IEnumerable<Domain.Entities.OrderItem> orderItems = await this._orderItemAsyncRepository.ListAllAsync(query.OrderId);
			response.OrderItems = this._mapper.Map<IEnumerable<OrderItemDto>>(orderItems);

			if (orderItems.Any() == false)
			{
				response.Success = false;
				response.Message = "No OrderItems Found";
			}

			return response;
		}
	}
}