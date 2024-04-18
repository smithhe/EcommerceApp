using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Order.Commands.UpdateOrder;
using Ecommerce.Application.Features.Order.Queries.GetOrderById;
using Ecommerce.Domain.Constants;
using Ecommerce.Shared.Enums;
using Ecommerce.Shared.Responses.Order;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.Order.Queries.GetOrderAfterSuccessfulCheckout
{
    /// <summary>
    /// A <see cref="Mediator"/> request handler for <see cref="GetOrderAfterSuccessfulCheckoutQuery"/>
    /// </summary>
    public class GetOrderAfterSuccessfulCheckoutQueryHandler : IRequestHandler<GetOrderAfterSuccessfulCheckoutQuery,
        GetOrderAfterSuccessfulCheckoutResponse>
    {
        private readonly ILogger<GetOrderAfterSuccessfulCheckoutQueryHandler> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetOrderAfterSuccessfulCheckoutQueryHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        public GetOrderAfterSuccessfulCheckoutQueryHandler(ILogger<GetOrderAfterSuccessfulCheckoutQueryHandler> logger,
            IMediator mediator)
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        /// Handles the <see cref="GetOrderAfterSuccessfulCheckoutQuery"/> request
        /// </summary>
        /// <param name="query">The <see cref="GetOrderAfterSuccessfulCheckoutQuery"/> request to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// Returns a <see cref="GetOrderAfterSuccessfulCheckoutResponse"/> with Success being <c>true</c> if the Order was found and the status was updated to processing;
        /// Success will be false if the Order was not found or the status was not updated to processing;
        /// Message will contain the message to display to the user.
        /// </returns>
        public async Task<GetOrderAfterSuccessfulCheckoutResponse> Handle(GetOrderAfterSuccessfulCheckoutQuery query,
            CancellationToken cancellationToken)
        {
            //Log the request
            this._logger.LogInformation("Handling request to get the Order after a successful checkout");

            //Create a new response
            GetOrderAfterSuccessfulCheckoutResponse response = new GetOrderAfterSuccessfulCheckoutResponse
            {
                Success = true,
                Message = OrderConstants._getOrderAfterSuccessfulCheckoutSuccessMessage
            };

            //Lookup the Order
            GetOrderByIdResponse getOrderByIdResponse = await this._mediator.Send(new GetOrderByIdQuery { Id = query.Id }, cancellationToken);

            //Check if we have an order
            if (getOrderByIdResponse.Success == false || getOrderByIdResponse.Order == null)
            {
                //Log the error and return false
                this._logger.LogError("Failed to get the Order by Id");
                return new GetOrderAfterSuccessfulCheckoutResponse { Success = false, Message = OrderConstants._getOrderAfterSuccessfulCheckoutErrorMessage };
            }

            //Check if the order was already processed
            if (getOrderByIdResponse.Order.Status != OrderStatus.Pending)
            {
                //Log the error and return false
                this._logger.LogError("The Order has already been processed");
                return new GetOrderAfterSuccessfulCheckoutResponse { Success = false, Message = OrderConstants._getOrderAfterSuccessfulCheckoutErrorMessage };
            }

            //Update the order status to processing
            getOrderByIdResponse.Order.Status = OrderStatus.Processing;
            response.Order = getOrderByIdResponse.Order;

            //Save the updated order
            UpdateOrderResponse updateOrderResponse = await this._mediator.Send(new UpdateOrderCommand
            {
                OrderToUpdate = getOrderByIdResponse.Order,
                UserName = "System"
            }, cancellationToken);

            //Check if the order was updated
            if (updateOrderResponse.Success == false)
            {
                //Log the error and return false
                this._logger.LogError("Failed to update the Order status to Processing");
                return new GetOrderAfterSuccessfulCheckoutResponse { Success = false, Message = OrderConstants._getOrderAfterSuccessfulCheckoutErrorMessage };
            }

            //Return the response
            return response;
        }
    }
}