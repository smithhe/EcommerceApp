using AutoMapper;
using Ecommerce.Application.Validators.OrderItem;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.OrderItem;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants;
using Ecommerce.Shared.Extensions;

namespace Ecommerce.Application.Features.OrderItem.Commands.CreateOrderItem
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="CreateOrderItemCommand"/>
	/// </summary>
	public class CreateOrderItemCommandHandler : IRequestHandler<CreateOrderItemCommand, CreateOrderItemResponse>
	{
		private readonly ILogger<CreateOrderItemCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IOrderItemAsyncRepository _orderItemAsyncRepository;
		private readonly IMediator _mediator;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateOrderItemCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="orderItemAsyncRepository">The <see cref="IOrderItemAsyncRepository"/> instance used for data access for <see cref="OrderItem"/> entities.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		public CreateOrderItemCommandHandler(ILogger<CreateOrderItemCommandHandler> logger, IMapper mapper, IOrderItemAsyncRepository orderItemAsyncRepository,
			IMediator mediator)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._orderItemAsyncRepository = orderItemAsyncRepository;
			this._mediator = mediator;
		}
		
		/// <summary>
		/// Handles the <see cref="CreateOrderItemCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="CreateOrderItemCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="CreateOrderItemResponse"/> with Success being <c>true</c> if the <see cref="Order"/> was created;
		/// Success will be <c>false</c> if validation of the command fails or Sql fails to create the <see cref="Order"/>.
		/// Message will contain an error message if Success is <c>false</c>.
		/// Validation Errors will be populated with errors to present if validation fails.
		/// OrderItem will contain the new <see cref="OrderItemDto"/> if creation was successful.
		/// </returns>
		public async Task<CreateOrderItemResponse> Handle(CreateOrderItemCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to create a new order item");

			//Create the response object
			CreateOrderItemResponse response = new CreateOrderItemResponse { Success = true, Message = string.Empty };
			
			//Check if username is null or empty
			if (string.IsNullOrEmpty(command.UserName))
			{
				this._logger.LogWarning("UserName was null or empty in command, returning failed response");
				response.Success = false;
				response.Message = OrderItemConstants._createUserNameErrorMessage;
				return response;
			}
			
			//Validate the dto that was passed in the command
			CreateOrderItemValidator validator = new CreateOrderItemValidator(this._mediator);
			ValidationResult validationResult = await validator.ValidateAsync(command, cancellationToken);

			//Check for validation errors
			if (validationResult.Errors.Count > 0)
			{
				this._logger.LogWarning("Command failed validation, returning validation errors");
				
				response.Success = false;
				response.Message = OrderItemConstants._genericValidationErrorMessage;
				foreach (ValidationFailure validationResultError in validationResult.Errors)
				{
					response.ValidationErrors.Add(validationResultError.ErrorMessage);
				}

				return response;
			}
			
			//Valid Command
			Domain.Entities.OrderItem orderItemToCreate = this._mapper.Map<Domain.Entities.OrderItem>(command.OrderItemToCreate);
			orderItemToCreate.CreatedBy = command.UserName;
			orderItemToCreate.CreatedDate = DateTime.UtcNow.ToEst();
			
			//Add the new OrderItem to the database
			int newId = await this._orderItemAsyncRepository.AddAsync(orderItemToCreate);

			//Sql operation failed, return failed response
			if (newId == -1)
			{
				this._logger.LogWarning("Sql operation failed to create OrderItem, returning failed response");
				response.Success = false;
				response.Message = OrderItemConstants._createSqlErrorMessage;
				return response;
			}
			
			//Get the new OrderItem from the database
			Domain.Entities.OrderItem? order = await this._orderItemAsyncRepository.GetByIdAsync(newId);
			response.OrderItem = this._mapper.Map<OrderItemDto?>(order);
			
			//Return the response
			return response;
		}
	}
}