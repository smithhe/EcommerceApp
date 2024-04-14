using AutoMapper;
using Ecommerce.Application.Validators.CartItem;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.CartItem;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants;

namespace Ecommerce.Application.Features.CartItem.Commands.CreateCartItem
{
	/// <summary>
	/// A <see cref="Mediator"/> request handler for <see cref="CreateCartItemCommand"/>
	/// </summary>
	public class CreateCartItemCommandHandler : IRequestHandler<CreateCartItemCommand, CreateCartItemResponse>
	{
		private readonly ILogger<CreateCartItemCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ICartItemRepository _cartItemRepository;
		private readonly IMediator _mediator;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateCartItemCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="cartItemRepository">The <see cref="ICartItemRepository"/> instance used for data access for <see cref="CartItem"/> entities.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		public CreateCartItemCommandHandler(ILogger<CreateCartItemCommandHandler> logger, IMapper mapper, ICartItemRepository cartItemRepository,
			IMediator mediator)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._cartItemRepository = cartItemRepository;
			this._mediator = mediator;
		}
		
		/// <summary>
		/// Handles the <see cref="CreateCartItemCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="CreateCartItemCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="CreateCartItemResponse"/> with Success being <c>true</c> if the <see cref="CartItem"/> was created;
		/// Success will be <c>false</c> if validation of the command fails or Sql fails to create the <see cref="CartItem"/>.
		/// Message will contain the message to display to the user.
		/// Validation Errors will be populated with errors to present if validation fails.
		/// CartItem will contain the new <see cref="CartItemDto"/> if creation was successful
		/// </returns>
		public async Task<CreateCartItemResponse> Handle(CreateCartItemCommand command, CancellationToken cancellationToken)
		{
			//Log the request
			this._logger.LogInformation("Handling request to create a new CartItem");
			
			//Create the response object
			CreateCartItemResponse response = new CreateCartItemResponse { Success = true, Message = CartItemConstants._createSuccessMessage };
			
			//Check if the dto is null
			if (command.CartItemToCreate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = CartItemConstants._createErrorMessage;
				return response;
			}
			
			//Check if username is null or empty
			if (string.IsNullOrEmpty(command.UserName))
			{
				this._logger.LogWarning("UserName was null or empty in command, returning failed response");
				response.Success = false;
				response.Message = CartItemConstants._createErrorMessage;
				return response;
			}
			
			//Validate the dto that was passed in the command
			CreateCartItemValidator validator = new CreateCartItemValidator(this._cartItemRepository, this._mediator);
			ValidationResult validationResult = await validator.ValidateAsync(command, cancellationToken);
			
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
			
			//Valid Command
			Domain.Entities.CartItem cartItemToCreate = this._mapper.Map<Domain.Entities.CartItem>(command.CartItemToCreate);
			cartItemToCreate.CreatedBy = command.UserName;
			cartItemToCreate.CreatedDate = DateTime.Now;

			//Attempt to create the cart item
			int newId = await this._cartItemRepository.AddAsync(cartItemToCreate);
			
			//If the create failed, return a response
			if (newId == -1)
			{
				this._logger.LogWarning("Sql returned -1, returning failed response");
				
				response.Success = false;
				response.Message = CartItemConstants._createErrorMessage;
				
				return response;
			}
			
			//Get the new cart item
			Domain.Entities.CartItem? cartItem = await this._cartItemRepository.GetByIdAsync(newId);
			response.CartItem = this._mapper.Map<CartItemDto?>(cartItem);
			
			//Return the response
			return response;
		}
	}
}