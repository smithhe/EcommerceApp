using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Order.Commands.AddPayPalRequestId;
using Ecommerce.Application.Features.PayPal.Commands.CreatePayPalReturnKey;
using Ecommerce.PayPal.Contracts;
using Ecommerce.Shared.Requests.PayPal;
using Ecommerce.Shared.Responses.PayPal;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.PayPal.Commands.CreatePayPalOrder
{
    /// <summary>
    /// A <see cref="Mediator"/> request handler for <see cref="CreatePayPalOrderCommand"/>
    /// </summary>
    public class CreatePayPalOrderCommandHandler : IRequestHandler<CreatePayPalOrderCommand, CreatePayPalOrderResponse>
    {
        private readonly ILogger<CreatePayPalOrderCommandHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IPaypalClientService _paypalClientService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePayPalOrderCommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        /// <param name="paypalClientService">The <see cref="IPaypalClientService"/> instance for handling PayPal Api Requests</param>
        public CreatePayPalOrderCommandHandler(ILogger<CreatePayPalOrderCommandHandler> logger, IMediator mediator, 
            IPaypalClientService paypalClientService)
        {
            this._logger = logger;
            this._mediator = mediator;
            this._paypalClientService = paypalClientService;
        }
        
        /// <summary>
        /// Handles the <see cref="CreatePayPalOrderCommand"/> command
        /// </summary>
        /// <param name="command">The <see cref="CreatePayPalOrderCommand"/> command to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// Success will be <c>false</c> if validation of the command fails.
        /// Message will contain the error to display if Success is <c>false</c>;
        /// RedirectUrl will contain the url to redirect the user to in the UI to complete the PayPal Order
        /// </returns>
        public async Task<CreatePayPalOrderResponse> Handle(CreatePayPalOrderCommand command, CancellationToken cancellationToken)
        {
            //Log the request
            this._logger.LogInformation("Handling request to create a new PayPal order");
            
            //Create the response
            CreatePayPalOrderResponse response = new CreatePayPalOrderResponse
            {
                Success = false,
                Message = "Failed to create PayPal Order"
            };
            
            //Verify the order is not null and has items
            if (command.Order.OrderItems == null || command.Order.OrderItems.Any() == false)
            {
                response.Message = "Order must have items to create a PayPal Order";
                return response;
            }
            
            //Create a new Guid for the PayPal request to help ensure Idempotency
            Guid payPalRequestId = Guid.NewGuid();
            command.Order.PayPalRequestId = payPalRequestId;
            
            //Create the order key for the PayPal order
            this._logger.LogInformation("Creating PayPal return key for order");
            string? returnKey = await this._mediator.Send(new CreatePayPalReturnKeyCommand
            {
                OrderId = command.Order.Id
            }, cancellationToken);
            
            //Check if the return key was created
            if (string.IsNullOrEmpty(returnKey))
            {
                this._logger.LogError("Failed to create PayPal return key for order");
                response.Message = "Failed to create order with PayPal";
                return response;
            }
            
            //Create the PayPal Order
            response = await this._paypalClientService.CreateOrder(new CreatePayPalOrderRequest
            {
                Order = command.Order,
                ReturnKey = returnKey
            });

            //Check for success in creating the paypal order
            if (response.Success)
            {
                //Add the PayPalRequestId to the order
                this._logger.LogInformation("Adding PayPalRequestId to order");
                await this._mediator.Send(new AddPayPalRequestIdCommand
                {
                    OrderId = command.Order.Id,
                    PayPalRequestId = payPalRequestId
                }, cancellationToken);
            }

            return response;
        }
    }
}