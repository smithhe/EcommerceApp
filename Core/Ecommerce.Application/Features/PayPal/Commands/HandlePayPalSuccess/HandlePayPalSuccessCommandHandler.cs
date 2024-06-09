using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.CartItem.Commands.DeleteUserCartItems;
using Ecommerce.Application.Features.Order.Commands.UpdateOrder;
using Ecommerce.Application.Features.PayPal.Queries.GetOrderByReturnKey;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Enums;
using Ecommerce.Shared.Responses.CartItem;
using Ecommerce.Shared.Responses.Order;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.PayPal.Commands.HandlePayPalSuccess
{
    /// <summary>
    /// A <see cref="Mediator"/> request handler for <see cref="HandlePayPalSuccessCommand"/>
    /// </summary>
    public class HandlePayPalSuccessCommandHandler : IRequestHandler<HandlePayPalSuccessCommand, bool>
    {
        private readonly ILogger<HandlePayPalSuccessCommandHandler> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlePayPalSuccessCommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        public HandlePayPalSuccessCommandHandler(ILogger<HandlePayPalSuccessCommandHandler> logger, IMediator mediator)
        {
            this._logger = logger;
            this._mediator = mediator;
        }
        
        /// <summary>
        /// Handles the <see cref="HandlePayPalSuccessCommand"/> command
        /// </summary>
        /// <param name="command">The <see cref="HandlePayPalSuccessCommand"/> command to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// Returns <c>true</c> if the Ecommerce Order is updated successfully and the cart is emptied
        /// Returns <c>false</c> if the Ecommerce Order fails to update or the cart fails to empty
        /// </returns>
        public async Task<bool> Handle(HandlePayPalSuccessCommand command, CancellationToken cancellationToken)
        {
            //Log the request
            this._logger.LogInformation("Handling request to handle a successful return from PayPal");

            //Verify we have a return key
            if (string.IsNullOrEmpty(command.ReturnKey))
            {
                //Log the error and return false
                this._logger.LogError("Failed to get the return key from the command");
                return false;
            }
            
            //Get the Order from the PayPal return key
            OrderDto? order = await this._mediator.Send(new GetOrderByReturnKeyQuery { ReturnKey = command.ReturnKey }, cancellationToken);
            
            //Check if we have an order
            if (order == null)
            {
                //Log the error and return false
                this._logger.LogError("Failed to get the order from the return key");
                return false;
            }
            
            //Update the order status to pending
            order.Status = OrderStatus.Pending;
            
            //Update the order in the database
            UpdateOrderResponse updateOrderResponse = await this._mediator.Send(new UpdateOrderCommand
            {
                OrderToUpdate = order,
                UserName = "System"
            }, cancellationToken);
            
            //Check if the update was successful
            if (updateOrderResponse.Success == false)
            {
                //Log the error and return false
                this._logger.LogError("Failed to update the order in the database");
                return false;
            }
            
            //Return success
            return true;
        }
    }
}