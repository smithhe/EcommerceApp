using AutoMapper;
using Ecommerce.Application.Validators.CartItem;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.CartItem;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

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
		private readonly IProductAsyncRepository _productAsyncRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateCartItemCommandHandler"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
		/// <param name="cartItemRepository">The <see cref="ICartItemRepository"/> instance used for data access for <see cref="CartItem"/> entities.</param>
		/// <param name="productAsyncRepository">The <see cref="IProductAsyncRepository"/> instance used for data access for <see cref="Product"/> entities.</param>
		public UpdateCartItemCommandHandler(ILogger<UpdateCartItemCommandHandler> logger, IMapper mapper, ICartItemRepository cartItemRepository,
			IProductAsyncRepository productAsyncRepository)
		{
			this._logger = logger;
			this._mapper = mapper;
			this._cartItemRepository = cartItemRepository;
			this._productAsyncRepository = productAsyncRepository;
		}
		
		/// <summary>
		/// Handles the <see cref="UpdateCartItemCommand"/> request
		/// </summary>
		/// <param name="command">The <see cref="UpdateCartItemCommand"/> request to be handled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		/// <returns>
		/// A <see cref="UpdateCartItemResponse"/> with Success being <c>true</c> if the <see cref="CartItem"/> was updated;
		/// Success will be <c>false</c> if no <see cref="CartItem"/> is found or validation of the command fails;
		/// Message will contain the error to display if Success is <c>false</c>;
		/// Validation Errors will be populated with errors to present if validation fails
		/// </returns>
		public async Task<UpdateCartItemResponse> Handle(UpdateCartItemCommand command, CancellationToken cancellationToken)
		{
			this._logger.LogInformation("Handling request to update an existing cart item");
			
			UpdateCartItemResponse response = new UpdateCartItemResponse { Success = true, Message = "Review Updated Successfully" };
			
			//Check if the dto is null
			if (command.CartItemToUpdate == null)
			{
				this._logger.LogWarning("Dto was null in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a CartItem to update";
				return response;
			}
			
			//Check if username is null or empty
			if (string.IsNullOrEmpty(command.UserName))
			{
				this._logger.LogWarning("UserName was null in command, returning failed response");
				response.Success = false;
				response.Message = "Must provide a UserName to create";
				return response;
			}

			//Validate the dto that was passed in the command
			UpdateCartItemValidator validator = new UpdateCartItemValidator(this._cartItemRepository, this._productAsyncRepository);
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
			
			bool success = await this._cartItemRepository.UpdateAsync(this._mapper.Map<Domain.Entities.CartItem>(command.CartItemToUpdate));
			
			if (success == false)
			{
				response.Success = false;
				response.Message = "Failed to update the CartItem";
			}
			
			return response;
		}
	}
}