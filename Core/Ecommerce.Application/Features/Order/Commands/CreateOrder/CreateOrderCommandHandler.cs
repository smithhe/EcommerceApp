using AutoMapper;
using AutoMapper.Internal;
using Ecommerce.Application.Features.OrderItem.Commands.CreateOrderItem;
using Ecommerce.Application.Validators.Order;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Order;
using Ecommerce.Shared.Responses.OrderItem;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Product.Queries.GetProductById;
using Ecommerce.Shared.Enums;
using Ecommerce.Shared.Responses.Product;

namespace Ecommerce.Application.Features.Order.Commands.CreateOrder
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="CreateOrderCommand"/>
	/// </summary>
	public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
	{
		private readonly ILogger<CreateOrderCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IOrderAsyncRepository _orderAsyncRepository;
		private readonly IMediator _mediator;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateOrderCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="orderAsyncRepository">The <see cref="IOrderAsyncRepository"/> instance used for data access for <see cref="Order"/> entities.</param>
		public CreateOrderCommandHandler(ILogger<CreateOrderCommandHandler> logger, IMapper mapper, IMediator mediator,
			IOrderAsyncRepository orderAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._orderAsyncRepository = orderAsyncRepository;
			this._mediator = mediator;
		}
		
		/// <summary>
		/// Handles the <see cref="CreateOrderCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="CreateOrderCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="CreateOrderResponse"/> with Success being <c>true</c> if the <see cref="Order"/> was created;
		/// Success will be <c>false</c> if validation of the command fails or Sql fails to create the <see cref="Order"/>.
		/// Message will contain the error to display if Success is <c>false</c>;
		/// Validation Errors will be populated with errors to present if validation fails
		/// Order will contain the new <see cref="OrderDto"/> if creation was successful
		/// </returns>
		public async Task<CreateOrderResponse> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to create a new order");

			CreateOrderResponse response = new CreateOrderResponse { Success = true, Message = "Order Successfully Created" };
			
			//Check if the CartItems is null or empty
			if (command.CartItems == null || command.CartItems.Any() == false)
			{
				this._logger.LogWarning("CartItems was null or empty in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a CartItems to create";
				return response;
			}
			
			//Check if username is null or empty
			if (string.IsNullOrEmpty(command.UserName))
			{
				this._logger.LogWarning("UserName was null or empty in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a UserName to create";
				return response;
			}
			
			//Check if the userid is null or empty
			if (command.UserId == null || command.UserId == Guid.Empty)
			{
				this._logger.LogWarning("UserId was null or empty in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a UserId to create";
				return response;
			}
			
			//Create a new order
			OrderDto newOrder = new OrderDto { Status = OrderStatus.Created };
			List<OrderItemDto> orderItems = new List<OrderItemDto>();
			double total = 0;
			
			//Create order items from the cart items
			foreach (CartItemDto cartItem in command.CartItems)
			{
				//Get the price of the product
				ProductDto? cartItemProduct = await this.GetProduct(cartItem.ProductId);
				
				//Verify we got a valid price back
				if (cartItemProduct == null)
				{
					//Return a failed response due to error getting product price
					response.Success = false;
					response.Message = "Failed to get product during order create";
					return response;
				}
				
				//Create the order item
				OrderItemDto orderItem = new OrderItemDto
				{
					ProductName = cartItemProduct.Name,
					ProductDescription = cartItemProduct.Description,
					ProductSku = cartItemProduct.Id.ToString(),
					Quantity = cartItem.Quantity,
					Price = cartItemProduct.Price
				};
				
				//Add to the total
				total += cartItemProduct.Price * cartItem.Quantity;
				
				orderItems.Add(orderItem);
			}
			
			//Add the order items and total to the order
			newOrder.OrderItems = orderItems.ToArray();
			newOrder.UserId = (Guid)command.UserId;
			newOrder.Total = total;
			
			//Validate the dto that was passed in the command
			CreateOrderValidator validator = new CreateOrderValidator();
			ValidationResult validationResult = await validator.ValidateAsync(newOrder, cancellationToken);
			
			//Check for validation errors
			if (validationResult.Errors.Count > 0)
			{
				this._logger.LogWarning("Command failed validation, returning validation errors");
				
				response.Success = false;
				response.Message = "Command was invalid";
				foreach (ValidationFailure validationResultError in validationResult.Errors)
				{
					response.ValidationErrors.Add(validationResultError.ErrorMessage);
				}

				return response;
			}
			
			//Order is valid, map the dto to the entity
			Domain.Entities.Order orderToCreate = this._mapper.Map<Domain.Entities.Order>(newOrder);
			orderToCreate.CreatedBy = command.UserName;
			orderToCreate.CreatedDate = DateTime.Now;
			
			//Create the order
			int newId = await this._orderAsyncRepository.AddAsync(orderToCreate);
			
			//Sql operation failed
			if (newId == -1)
			{
				response.Success = false;
				response.Message = "Failed to add new Order";
				return response;
			}
			
			Domain.Entities.Order? order = await this._orderAsyncRepository.GetByIdAsync(newId);
			
			//Verify the order was created
			if (order == null)
			{
				//Log the error
				this._logger.LogError("Failed to create Order in SQL");
				
				//Return a failed response
				response.Success = false;
				response.Message = "Failed to create Order";
				return response;
			}
			
			//Map the order to the response
			response.Order = this._mapper.Map<OrderDto?>(order);

			//Create all the order items for the order
			foreach (OrderItemDto orderItem in newOrder.OrderItems)
			{
				//Update the order Id before sending the request
				orderItem.OrderId = newId;
				CreateOrderItemResponse orderItemResponse = await this._mediator.Send(new CreateOrderItemCommand { OrderItemToCreate = orderItem }, cancellationToken);

				if (orderItemResponse.Success == false)
				{
					response.Success = false;
					response.Message = "Failed to create order items";
					
					//Delete the order since the items failed to create
					await this._orderAsyncRepository.DeleteAsync(order);
					
					if (orderItemResponse.ValidationErrors.Count > 0)
					{
						response.ValidationErrors.Concat(orderItemResponse.ValidationErrors);
					}

					break;
				}
			}

			return response;
		}

		private async Task<ProductDto?> GetProduct(int productId)
		{
			GetProductByIdResponse response = await this._mediator.Send(new GetProductByIdQuery() { Id = productId });
			
			return response.Product;
		}
	}
}