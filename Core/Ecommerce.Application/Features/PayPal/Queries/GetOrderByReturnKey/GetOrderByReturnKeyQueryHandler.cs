using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Order.Queries.GetOrderById;
using Ecommerce.Domain.Infrastructure;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.PayPal.Queries.GetOrderByReturnKey
{
    /// <summary>
    /// A <see cref="Mediator"/> request handler for <see cref="GetOrderByReturnKeyQuery"/>
    /// </summary>
    public class GetOrderByReturnKeyQueryHandler : IRequestHandler<GetOrderByReturnKeyQuery, OrderDto?>
    {
        private readonly ILogger<GetOrderByReturnKeyQueryHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IOrderKeyRepository _orderKeyRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetOrderByReturnKeyQueryHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        /// <param name="orderKeyRepository">The <see cref="IOrderKeyRepository"/> instance used for data access for Order Keys</param>
        public GetOrderByReturnKeyQueryHandler(ILogger<GetOrderByReturnKeyQueryHandler> logger, IMediator mediator, IOrderKeyRepository orderKeyRepository)
        {
            this._logger = logger;
            this._mediator = mediator;
            this._orderKeyRepository = orderKeyRepository;
        }
        
        /// <summary>
        /// Handles the <see cref="GetOrderByReturnKeyQuery"/> request
        /// </summary>
        /// <param name="request">The <see cref="GetOrderByReturnKeyQuery"/> request to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// Returns the <see cref="Domain.Entities.Order"/> if the Order was found;
        /// Returns <c>null</c> if no Order with the specified return key is not found.
        /// </returns>
        public async Task<OrderDto?> Handle(GetOrderByReturnKeyQuery request, CancellationToken cancellationToken)
        {
            //Log the request
            this._logger.LogInformation("Handling request to get an existing Order by Return Key");
            
            //Lookup the Order Id by the return key
            OrderKey? orderKey = await this._orderKeyRepository.GetByReturnKeyAsync(request.ReturnKey);
            
            //Check if the order key was found or not
            if (orderKey == null)
            {
                this._logger.LogWarning($"No OrderKey found for the specified Return Key {request.ReturnKey}");
                return null;
            }
            
            //Get the Order by the Order Id
            GetOrderByIdResponse getOrderByIdResponse = await this._mediator.Send(new GetOrderByIdQuery { Id = orderKey.OrderId }, cancellationToken);
            
            //Check if the Order was found or not
            if (getOrderByIdResponse.Success == false || getOrderByIdResponse.Order == null)
            {
                this._logger.LogWarning($"No Order found for the specified Order Id {orderKey.OrderId}");
                return null;
            }
            
            //Return the Order
            return getOrderByIdResponse.Order;
        }
    }
}