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
		private readonly IProductAsyncRepository _productAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateCartItemCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="cartItemRepository">The <see cref="ICartItemRepository"/> instance used for data access for <see cref="CartItem"/> entities.</param>
		/// <param name="productAsyncRepository">The <see cref="IProductAsyncRepository"/> instance used for data access for <see cref="Product"/> entities.</param>
		public CreateCartItemCommandHandler(ILogger<CreateCartItemCommandHandler> logger, IMapper mapper, ICartItemRepository cartItemRepository,
			IProductAsyncRepository productAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._cartItemRepository = cartItemRepository;
			this._productAsyncRepository = productAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="CreateCartItemCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="CreateCartItemCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="CreateCartItemResponse"/> with Success being <c>true</c> if the <see cref="CartItem"/> was created;
		/// Success will be <c>false</c> if validation of the command fails or Sql fails to create the <see cref="CartItem"/>.
		/// Message will contain the error to display if Success is <c>false</c>.
		/// Validation Errors will be populated with errors to present if validation fails.
		/// CartItem will contain the new <see cref="CartItemDto"/> if creation was successful
		/// </returns>
		public async Task<CreateCartItemResponse> Handle(CreateCartItemCommand command, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to create a new CartItem");
			
			CreateCartItemResponse response = new CreateCartItemResponse { Success = true, Message = "Successfully Created CartItem" };
			
			//Check if the dto is null
			if (command.CartItemToCreate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a CartItem to create";
				return response;
			}
			
			//Check if username is null
			if (command.UserName == null)
			{
				this._logger.LogWarning("UserName was null in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a UserName to create";
				return response;
			}
			
			//Validate the dto that was passed in the command
			CreateCartItemValidator validator = new CreateCartItemValidator(this._cartItemRepository, this._productAsyncRepository);
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
			Domain.Entities.CartItem? newCartItem = this._mapper.Map<Domain.Entities.CartItem>(command.CartItemToCreate);
			newCartItem.CreatedBy = command.UserName;
			newCartItem.CreatedDate = DateTime.Now;

			int newId = await this._cartItemRepository.AddAsync(newCartItem);
			
			//Sql operation failed
			if (newId == -1)
			{
				response.Success = false;
				response.Message = "Failed to add new CartItem";
			}
			else
			{
				Domain.Entities.CartItem? cartItem = await this._cartItemRepository.GetByIdAsync(newId);
				response.CartItem = this._mapper.Map<CartItemDto?>(cartItem);
			}
			
			return response;
		}
	}
}