using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Order.Commands.DeleteOrder;
using Ecommerce.Application.Features.PayPal.Commands.DeletePayPalReturnKey;
using Ecommerce.Application.Features.PayPal.Queries.GetOrderByReturnKey;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.PayPal.Commands.CancelPayPalOrder
{
    /// <summary>
    /// A <see cref="Mediator"/> request handler for <see cref="CancelPayPalOrderCommand"/>
    /// </summary>
    public class CancelPayPalOrderCommandHandler : IRequestHandler<CancelPayPalOrderCommand, bool>
    {
        private readonly ILogger<CancelPayPalOrderCommandHandler> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelPayPalOrderCommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        public CancelPayPalOrderCommandHandler(ILogger<CancelPayPalOrderCommandHandler> logger, IMediator mediator)
        {
            this._logger = logger;
            this._mediator = mediator;
        }
        
        /// <summary>
        /// Handles the <see cref="CancelPayPalOrderCommand"/> command
        /// </summary>
        /// <param name="command">The <see cref="CancelPayPalOrderCommand"/> command to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// Returns <c>true</c> if the Ecommerce Order and the PayPal Order was successfully cancelled
        /// Returns <c>false</c> if the Ecommerce Order or the PayPal Order was not successfully cancelled
        /// </returns>
        public async Task<bool> Handle(CancelPayPalOrderCommand command, CancellationToken cancellationToken)
        {
            //Log the request
            this._logger.LogInformation("Handling request to cancel a PayPal order");

            //Get the Order from the PayPal return key
            OrderDto? order = await this._mediator.Send(new GetOrderByReturnKeyQuery { ReturnKey = command.ReturnKey }, cancellationToken);
            
            //Check if we have an order
            if (order == null)
            {
                //Log the error
                this._logger.LogError("Failed to get the order from the return key");
                return false;
            }
            
            //Delete the order from the database
            DeleteOrderResponse deleteOrderResponse = await this._mediator.Send(new DeleteOrderCommand { Order = order }, cancellationToken);
            
            //Check if the delete was successful
            if (deleteOrderResponse.Success == false)
            {
                //Log the error
                this._logger.LogError("Failed to delete the order from the database");
                return false;
            }
            
            //Delete the return key from the database
            bool deleteReturnKeyResponse = await this._mediator.Send(new DeletePayPalReturnKeyCommand { ReturnKey = command.ReturnKey }, cancellationToken);
            
            //Return the result of the delete
            return deleteReturnKeyResponse;
        }
    }
}