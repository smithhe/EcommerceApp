using AutoMapper;
using Ecommerce.Application.Validators.Order;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.Order;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Shared.Extensions;

namespace Ecommerce.Application.Features.Order.Commands.UpdateOrder
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="UpdateOrderCommand"/>
	/// </summary>
	public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, UpdateOrderResponse>
	{
		private readonly ILogger<UpdateOrderCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IOrderAsyncRepository _orderAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateOrderCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="orderAsyncRepository">The <see cref="IOrderAsyncRepository"/> instance used for data access for <see cref="Order"/> entities.</param>
		public UpdateOrderCommandHandler(ILogger<UpdateOrderCommandHandler> logger, IMapper mapper, IOrderAsyncRepository orderAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._orderAsyncRepository = orderAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="UpdateOrderCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="UpdateOrderCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="UpdateOrderResponse"/> with Success being <c>true</c> if the <see cref="Order"/> was updated;
		/// Success will be <c>false</c> if no <see cref="Order"/> is found or validation of the command fails.
		/// Message will contain the error to display if Success is <c>false</c>;
		/// Validation Errors will be populated with errors to present if validation fails
		/// </returns>
		public async Task<UpdateOrderResponse> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to update an existing order");

			UpdateOrderResponse response = new UpdateOrderResponse { Success = true, Message = "Order Updated Successfully" };
			
			//Check if the dto is null
			if (command.OrderToUpdate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a Order to update";
				return response;
			}
			
			//Check if username is null or empty
			if (string.IsNullOrEmpty(command.UserName))
			{
				this._logger.LogWarning("UserName was null or empty in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a UserName to update";
				return response;
			}
			
			//Validate the dto that was passed in the command
			UpdateOrderValidator validator = new UpdateOrderValidator();
			ValidationResult? validationResult = await validator.ValidateAsync(command, cancellationToken);
			
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
			Domain.Entities.Order orderToUpdate = this._mapper.Map<Domain.Entities.Order>(command.OrderToUpdate);
			orderToUpdate.LastModifiedBy = command.UserName;
			orderToUpdate.LastModifiedDate = DateTime.UtcNow.ToEst();
			
			bool success = await this._orderAsyncRepository.UpdateAsync(orderToUpdate);
			
			if (success == false)
			{
				response.Success = false;
				response.Message = "Failed to update the Order";
			}

			return response;
		}
	}
}