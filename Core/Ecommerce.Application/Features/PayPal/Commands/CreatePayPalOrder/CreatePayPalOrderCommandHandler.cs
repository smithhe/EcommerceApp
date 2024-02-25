using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Order.Commands.AddPayPalRequestId;
using Ecommerce.Application.Features.Product.Queries.GetProductById;
using Ecommerce.PayPal.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.PayPal;
using Ecommerce.Shared.Responses.PayPal;
using Ecommerce.Shared.Responses.Product;
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
            if (command.Order?.OrderItems == null || command.Order.OrderItems.Any() == false)
            {
                response.Message = "Order must have items to create a PayPal Order";
                return response;
            }
            
            //Get the list of products from the order
            List<ProductDto> orderProducts = new List<ProductDto>();
            foreach (OrderItemDto orderItem in command.Order.OrderItems)
            {
                ProductDto? product = await this.GetProductInfo(orderItem.ProductId);
                
                //If the product is null, log the error and return the response
                if (product == null)
                {
                    this._logger.LogError($"Failed to get product info for product for order item {orderItem.ProductId}");
                    response.Message = "Failed to get product info for product in PayPal order create";
                    return response;
                }
                
                orderProducts.Add(product);
            }
            
            //Create a new Guid for the PayPal request to help ensure Idempotency
            Guid payPalRequestId = Guid.NewGuid();
            command.Order.PayPalRequestId = payPalRequestId;
            
            //Create the PayPal Order
            response = await this._paypalClientService.CreateOrder(new CreatePayPalOrderRequest
            {
                Order = command.Order,
                OrderProducts = orderProducts.ToArray()
            });

            //Check for success in creating the paypal order
            if (response.Success)
            {
                //Add the PayPalRequestId to the order
                bool addedPayPalRequestId = await this._mediator.Send(new AddPayPalRequestIdCommand
                {
                    OrderId = command.Order.Id,
                    PayPalRequestId = payPalRequestId
                }, cancellationToken);
            }

            return response;
        }
        
        private async Task<ProductDto?> GetProductInfo(int productId)
        {
            GetProductByIdResponse response = await this._mediator.Send(new GetProductByIdQuery() { Id = productId });

            return response.Product;
        }
    }
}