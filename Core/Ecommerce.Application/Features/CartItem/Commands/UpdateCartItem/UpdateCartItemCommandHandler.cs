using AutoMapper;
using Ecommerce.Application.Validators.CartItem;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.CartItem;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.Shared.Extensions;

namespace Ecommerce.Application.Features.CartItem.Commands.UpdateCartItem
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="UpdateCartItemCommand"/>
	/// </summary>
	public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, UpdateCartItemResponse>
	{
		private readonly ILogger<UpdateCartItemCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ICartItemRepository _cartItemRepository;
		private readonly IMediator _mediator;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateCartItemCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="cartItemRepository">The <see cref="ICartItemRepository"/> instance used for data access for <see cref="CartItem"/> entities.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		public UpdateCartItemCommandHandler(ILogger<UpdateCartItemCommandHandler> logger, IMapper mapper, ICartItemRepository cartItemRepository,
			IMediator mediator)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._cartItemRepository = cartItemRepository;
			this._mediator = mediator;
		}
		
		/// <summary>
		/// Handles the <see cref="UpdateCartItemCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="UpdateCartItemCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="UpdateCartItemResponse"/> with Success being <c>true</c> if the <see cref="CartItem"/> was updated;
		/// Success will be <c>false</c> if no <see cref="CartItem"/> is found or validation of the command fails;
		/// Message will contain the message to display to the user.
		/// Validation Errors will be populated with errors to present if validation fails
		/// </returns>
		public async Task<UpdateCartItemResponse> Handle(UpdateCartItemCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to update an existing cart item");
			
			//Create the response object
			UpdateCartItemResponse response = new UpdateCartItemResponse { Success = true, Message = CartItemConstants._updateSuccessMessage };
			
			//Check if the dto is null
			if (command.CartItemToUpdate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = CartItemConstants._updateErrorMessage;
				return response;
			}
			
			//Check if username is null or empty
			if (string.IsNullOrEmpty(command.UserName))
			{
				this._logger.LogWarning("UserName was null or empty in command, returning failed response");
				response.Success = false;
				response.Message = CartItemConstants._updateErrorMessage;
				return response;
			}

			//Validate the dto that was passed in the command
			UpdateCartItemValidator validator = new UpdateCartItemValidator(this._cartItemRepository, this._mediator);
			ValidationResult? validationResult = await validator.ValidateAsync(command, cancellationToken);
			
			//Check for validation errors
			if (validationResult.Errors.Count > 0)
			{
				this._logger.LogWarning("Command failed validation, returning validation errors");
				
				response.Success = false;
				response.Message = CartItemConstants._genericValidationErrorMessage;
				foreach (ValidationFailure validationResultError in validationResult.Errors)
				{
					response.ValidationErrors.Add(validationResultError.ErrorMessage);
				}

				return response;
			}

			//Valid command
			Domain.Entities.CartItem cartItemToUpdate = this._mapper.Map<Domain.Entities.CartItem>(command.CartItemToUpdate);
			cartItemToUpdate.LastModifiedBy = command.UserName;
			cartItemToUpdate.LastModifiedDate = DateTime.UtcNow.ToEst();
			
			//Update the cart item
			bool success = await this._cartItemRepository.UpdateAsync(cartItemToUpdate);
			
			//If the update failed, update to a failed response
			if (success == false)
			{
				this._logger.LogWarning("Sql returned false, returning failed response");
				response.Success = false;
				response.Message = CartItemConstants._updateErrorMessage;
			}
			
			//Return the response
			return response;
		}
	}
}