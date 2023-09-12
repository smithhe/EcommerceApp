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
		private readonly IProductAsyncRepository _productAsyncRepository;
		private readonly IOrderAsyncRepository _orderAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateOrderItemCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="orderItemAsyncRepository">The <see cref="IOrderItemAsyncRepository"/> instance used for data access for <see cref="OrderItem"/> entities.</param>
		/// <param name="productAsyncRepository">The <see cref="IProductAsyncRepository"/> instance used for data access for <see cref="Product"/> entities.</param>
		/// <param name="orderAsyncRepository">The <see cref="IOrderAsyncRepository"/> instance used for data access for <see cref="Order"/> entities.</param>
		public CreateOrderItemCommandHandler(ILogger<CreateOrderItemCommandHandler> logger, IMapper mapper, IOrderItemAsyncRepository orderItemAsyncRepository,
			IProductAsyncRepository productAsyncRepository, IOrderAsyncRepository orderAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._orderItemAsyncRepository = orderItemAsyncRepository;
			this._productAsyncRepository = productAsyncRepository;
			this._orderAsyncRepository = orderAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="CreateOrderItemCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="CreateOrderItemCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="CreateOrderItemResponse"/> with Success being <c>true</c> if the <see cref="Order"/> was created;
		/// Success will be <c>false</c> if validation of the command fails or Sql fails to create the <see cref="Order"/>.
		/// Message will contain the error to display if Success is <c>false</c>;
		/// Validation Errors will be populated with errors to present if validation fails
		/// OrderItem will contain the new <see cref="OrderItemDto"/> if creation was successful
		/// </returns>
		public async Task<CreateOrderItemResponse> Handle(CreateOrderItemCommand command, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to create a new order item");

			CreateOrderItemResponse response = new CreateOrderItemResponse { Success = true, Message = "OrderItem Successfully Created" };
			
			//Check if username is null or empty
			if (string.IsNullOrEmpty(command.UserName))
			{
				this._logger.LogWarning("UserName was null or empty in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a UserName to create";
				return response;
			}
			
			//Validate the dto that was passed in the command
			CreateOrderItemValidator validator = new CreateOrderItemValidator(this._productAsyncRepository, this._orderAsyncRepository);
			ValidationResult validationResult = await validator.ValidateAsync(command, cancellationToken);

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
			
			//Valid Command
			Domain.Entities.OrderItem orderItemToCreate = this._mapper.Map<Domain.Entities.OrderItem>(command.OrderItemToCreate);
			orderItemToCreate.CreatedBy = command.UserName;
			orderItemToCreate.CreatedDate = DateTime.Now;
			
			int newId = await this._orderItemAsyncRepository.AddAsync(orderItemToCreate);

			//Sql operation failed
			if (newId == -1)
			{
				response.Success = false;
				response.Message = "Failed to add new Order Item";
			}
			else
			{
				Domain.Entities.OrderItem? order = await this._orderItemAsyncRepository.GetByIdAsync(newId);
				response.OrderItem = this._mapper.Map<OrderItemDto?>(order);
			}
			
			return response;
		}
	}
}