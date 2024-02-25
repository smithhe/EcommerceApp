using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Order.Commands.UpdateOrder;
using Ecommerce.Application.Features.Order.Queries.GetOrderById;
using Ecommerce.Shared.Responses.Order;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.Order.Commands.AddPayPalRequestId
{
    /// <summary>
    /// A <see cref="Mediator"/> request handler for <see cref="AddPayPalRequestIdCommand"/>
    /// </summary>
    public class AddPayPalRequestIdCommandHandler : IRequestHandler<AddPayPalRequestIdCommand, bool>
    {
        private readonly ILogger<AddPayPalRequestIdCommandHandler> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddPayPalRequestIdCommand"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        public AddPayPalRequestIdCommandHandler(ILogger<AddPayPalRequestIdCommandHandler> logger, IMediator mediator)
        {
            this._logger = logger;
            this._mediator = mediator;
        }
        
        /// <summary>
        /// Handles the <see cref="AddPayPalRequestIdCommand"/> request
        /// </summary>
        /// <param name="command">The <see cref="AddPayPalRequestIdCommand"/> request to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// True if the PayPal RequestId was added to the <see cref="Order"/> successfully;
        /// False if the update fails for any reason
        /// </returns>
        public async Task<bool> Handle(AddPayPalRequestIdCommand command, CancellationToken cancellationToken)
        {
            //Log the request
            this._logger.LogInformation("Handling request to add PayPal RequestId to Order");
            
            //Get the order
            GetOrderByIdResponse getOrderByIdResponse = await this._mediator.Send(new GetOrderByIdQuery { Id = command.OrderId }, cancellationToken);
            
            if (getOrderByIdResponse.Order == null)
            {
                this._logger.LogError($"Failed to find the order {command.OrderId} to add the PayPal RequestId to");
                return false;
            }
            
            //Update the order with the PayPal RequestId
            getOrderByIdResponse.Order.PayPalRequestId = command.PayPalRequestId;
            
            //Update the order
            //Use system as the username since only the system should be adding the PayPalRequestId
            UpdateOrderResponse updateOrderResponse = await this._mediator.Send(new UpdateOrderCommand { OrderToUpdate = getOrderByIdResponse.Order, UserName = "System" }, cancellationToken);
            
            //Return the result
            return updateOrderResponse.Success;
        }
    }
}