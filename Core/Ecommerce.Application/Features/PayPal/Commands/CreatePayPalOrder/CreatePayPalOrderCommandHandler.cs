using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Product.Queries.GetProductById;
using Ecommerce.PayPal.Contracts;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.PayPal;
using Ecommerce.Shared.Responses.PayPal;
using Ecommerce.Shared.Responses.Product;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.PayPal.Commands.CreatePayPalOrder
{
    public class CreatePayPalOrderCommandHandler : IRequestHandler<CreatePayPalOrderCommand, CreatePayPalOrderResponse>
    {
        private readonly ILogger<CreatePayPalOrderCommandHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IPaypalClientService _paypalClientService;

        public CreatePayPalOrderCommandHandler(ILogger<CreatePayPalOrderCommandHandler> logger, IMediator mediator, 
            IPaypalClientService paypalClientService)
        {
            this._logger = logger;
            this._mediator = mediator;
            this._paypalClientService = paypalClientService;
        }
        
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
                
                if (product == null)
                {
                    this._logger.LogError($"Failed to get product info for product for order item {orderItem.ProductId}");
                    response.Message = "Failed to get product info for product in PayPal order create";
                    return response;
                }
                
                orderProducts.Add(product);
            }
            
            //Create the PayPal Order
            response = await this._paypalClientService.CreateOrder(new CreatePayPalOrderRequest
            {
                Order = command.Order,
                OrderProducts = orderProducts.ToArray()
            });

            return response;
        }
        
        private async Task<ProductDto?> GetProductInfo(int productId)
        {
            GetProductByIdResponse response = await this._mediator.Send(new GetProductByIdQuery() { Id = productId });

            return response.Product;
        }
    }
}